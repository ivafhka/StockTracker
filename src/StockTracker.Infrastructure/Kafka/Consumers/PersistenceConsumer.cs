using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StockTracker.Application.Interfaces;
using StockTracker.Domain.Entities;
using StockTracker.Domain.Interfaces;
using StockTracker.Domain.ValueObjects;
using StockTracker.Infrastructure.Kafka.Configuration;
using StockTracker.Infrastructure.Kafka.Models;

namespace StockTracker.Infrastructure.Kafka.Consumers
{
    internal class PersistenceConsumer : BaseConsumer<TickMessage>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public PersistenceConsumer(
            IOptions<KafkaOptions> options,
            ILogger<PersistenceConsumer> logger,
            IServiceScopeFactory scopeFactory)
            : base(options, logger)
        {
            _scopeFactory = scopeFactory;
        }

        protected override string Topic => kafkaTopics.StockTicks;
        protected override string ConsumerName => "persistence";

        protected override async Task HandleMessageAsync(TickMessage message, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var repository = scope.ServiceProvider.GetRequiredService<IPriceTickRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            try
            {
                var ticker = Ticker.Create(message.Ticker);
                var price = Money.Create(message.Price, message.Currency);
                var tick = PriceTick.Create(ticker, price, message.Volume, message.RecordedAt);

                await repository.AddBatchAsync(new[] { tick }, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                Logger.LogDebug("Persisted tick {Ticker} {Price} {Currency}",
                    message.Ticker, message.Price, message.Currency);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed, to persist tick for {Ticker}", message.Ticker);
                throw;
            }
        }
    }
}
