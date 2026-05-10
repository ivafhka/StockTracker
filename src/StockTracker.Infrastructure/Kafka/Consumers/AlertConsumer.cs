using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StockTracker.Application.Services;
using StockTracker.Domain.ValueObjects;
using StockTracker.Infrastructure.Kafka.Configuration;
using StockTracker.Infrastructure.Kafka.Models;

namespace StockTracker.Infrastructure.Kafka.Consumers;

public class AlertConsumer : BaseConsumer<TickMessage>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public AlertConsumer(
        IOptions<KafkaOptions> options,
        ILogger<AlertConsumer> logger,
        IServiceScopeFactory scopeFactory)
        : base(options, logger)
    {
        _scopeFactory = scopeFactory;
    }

    protected override string Topic => kafkaTopics.StockTicks;
    protected override string ConsumerName => "alerts";

    protected override async Task HandleMessageAsync(TickMessage message, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var alertService = scope.ServiceProvider.GetRequiredService<IAlertService>();

        try
        {
            var ticker = Ticker.Create(message.Ticker);
            var currentPrice = Money.Create(message.Price, message.Currency);

            var triggeredEvents = await alertService.EvaluateAlertsForPriceAsync(
                ticker,
                currentPrice,
                cancellationToken);

            if (triggeredEvents.Count > 0)
            {
                Logger.LogInformation(
                    "Triggered {Count} alerts for {Ticker} at price {Price}",
                    triggeredEvents.Count, message.Ticker, message.Price);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to evaluate alerts for {Ticker}", message.Ticker);
            throw;
        }
    }
}