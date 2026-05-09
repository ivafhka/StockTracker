using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StockTracker.Infrastructure.Kafka.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace StockTracker.Infrastructure.Kafka.Consumers
{
    public abstract class BaseConsumer<TMessage> : BackgroundService where TMessage : class
    {
        protected readonly KafkaOptions Options;
        protected readonly ILogger Logger;
        protected abstract string Topic { get; }
        protected abstract string ConsumerName { get; }

        protected BaseConsumer(IOptions<KafkaOptions> options, ILogger logger)
        {
            Options = options.Value;
            Logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = Options.BootstrapServers,
                GroupId = $"{Options.GroupIdPrefix}.{ConsumerName}.",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                SessionTimeoutMs = Options.ConsumerSessionTimeoutMs,
                EnableAutoOffsetStore = false
            };

            using var consumer = new ConsumerBuilder<string, string>(config)
                .SetErrorHandler((_, e) => Logger.LogError("Kafka consumer error: {Reason}", e.Reason))
                .Build();

            consumer.Subscribe(Topic);

            Logger.LogInformation("{ConsumerName} subscribed to {Topic}", ConsumerName, Topic);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var result = consumer.Consume(cancellationToken);

                        if (result?.Message?.Value is null)
                            continue;

                        var message = JsonSerializer.Deserialize<TMessage>(result.Message.Value);
                        if(message is null)
                        {
                            Logger.LogWarning("Failed ro deserialize message at offset {Offser}", result.Offset);
                            consumer.StoreOffset(result);
                            continue;
                        }

                        await HandleMessageAsync(message, cancellationToken);

                        consumer.StoreOffset(result);
                        consumer.Commit(result);
                    }
                    catch (ConsumeException ex)
                    {
                        Logger.LogError(ex, "Error consuming form {Topic}", Topic);
                        await Task.Delay(1000, cancellationToken);
                    }
                    catch( JsonException ex)
                    {
                        Logger.LogError(ex, "Failed to parse JSON message");
                    }
                }
            }
            catch ( OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                Logger.LogInformation("{ConsumerName} stopping", ConsumerName);
            }
            finally
            {
                consumer.Close();
            }
        }

        protected abstract Task HandleMessageAsync(TMessage message, CancellationToken cancellationToken);
    }
}
