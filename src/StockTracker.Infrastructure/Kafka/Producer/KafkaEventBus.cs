
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StockTracker.Application.Interfaces;
using StockTracker.Domain.Common;
using StockTracker.Infrastructure.Kafka.Configuration;
using System.Text.Json;

namespace StockTracker.Infrastructure.Kafka.Producer
{
    public class KafkaEventBus : IEventBus, IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<KafkaEventBus> _logger;

        public KafkaEventBus(IOptions<KafkaOptions> options, ILogger<KafkaEventBus> logger)
        {
            _logger = logger;

            var config = new ProducerConfig
            {
                BootstrapServers = options.Value.BootstrapServers,
                Acks = Acks.All,
                EnableIdempotence = true
            };

            _producer = new ProducerBuilder<string, string>(config)
                .SetErrorHandler((_, e) => _logger.LogError("Kafka event bus error: {Reason}", e.Reason))
                .Build();
        }

        public async Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
            where TEvent : IDomainEvents
        {
            var envelope = new EventEnvelope
            {
                EventType = typeof(TEvent).Name,
                EventId = domainEvent.EventId,
                OccuredAt = domainEvent.OccurredAt,
                Payload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType())
            };

            var json = JsonSerializer.Serialize(envelope);
            var message = new Message<string, string>
            {
                Key = domainEvent.EventId.ToString(),
                Value = json
            };

            try
            {
                await _producer.ProduceAsync(kafkaTopics.DomainEvents, message, cancellationToken);
                _logger.LogInformation("Published {EventType} with id {EventId}",
                    envelope.EventType, envelope.EventId);
            }
            catch(ProduceException<string,string> ex)
            {
                _logger.LogError(ex, "Failed to publish domain event {EventType}", envelope.EventType);
                throw;
            }
        }

        public void Dispose()
        {
            _producer.Flush(TimeSpan.FromSeconds(5));
            _producer.Dispose();
        }

        private class EventEnvelope
        {
            public string EventType { get; set; } = string.Empty;
            public Guid EventId { get; set; }
            public DateTime OccuredAt { get; set; }
            public string Payload { get; set; } = string.Empty;
        }
    }
}
