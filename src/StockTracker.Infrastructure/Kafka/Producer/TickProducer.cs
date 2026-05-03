
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StockTracker.Infrastructure.Kafka.Configuration;
using StockTracker.Infrastructure.Kafka.Models;
using System.Text.Json;

namespace StockTracker.Infrastructure.Kafka.Producer
{
    public class TickProducer : ITickProducer, IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<TickProducer> _logger;

        public TickProducer(IOptions<KafkaOptions> options, ILogger<TickProducer> logger)
        {
            _logger = logger;

            var config = new ProducerConfig
            {
                BootstrapServers = options.Value.BootstrapServers,
                LingerMs = options.Value.ProducerLingerMs,
                Acks = Acks.All,
                EnableIdempotence = true,
                CompressionType = CompressionType.Snappy
            };

            _producer = new ProducerBuilder<string, string>(config)
                .SetErrorHandler((_, e) => _logger.LogError("Kafka producer error: {Reason}", e.Reason))
                .Build();
        }
        public async Task ProduceAsync(TickMessage tick, CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(tick);
            var message = new Message<string, string>
            {
                Key = tick.Ticker,
                Value = json
            };

            try
            {
                await _producer.ProduceAsync(kafkaTopics.StockTicks, message, cancellationToken);
            }
            catch(ProduceException<string, string> ex)
            {
                _logger.LogError(ex, "Failed to produce tik for {Ticker}", tick.Ticker);
                throw;
            }
        }

        public void Dispose()
        {
            _producer.Flush(TimeSpan.FromSeconds(5));
            _producer.Dispose();
        }

        
    }
}
