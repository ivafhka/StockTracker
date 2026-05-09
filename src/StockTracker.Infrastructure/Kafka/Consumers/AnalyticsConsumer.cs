using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StockTracker.Application.Interfaces;
using StockTracker.Domain.Services;
using StockTracker.Infrastructure.Kafka.Configuration;
using StockTracker.Infrastructure.Kafka.Models;

namespace StockTracker.Infrastructure.Kafka.Consumers;

public class AnalyticsConsumer : BaseConsumer<TickMessage>
{
    private readonly ConcurrentDictionary<string, List<decimal>> _priceWindows = new();
    private readonly IServiceProvider _serviceProvider;
    private const int WindowSize = 50;
    private const int SmaPeriod = 20;
    private const int EmaPeriod = 12;

    public AnalyticsConsumer(
        IOptions<KafkaOptions> options,
        ILogger<AnalyticsConsumer> logger,
        IServiceProvider serviceProvider)
        : base(options, logger)
    {
        _serviceProvider = serviceProvider;
    }

    protected override string Topic => kafkaTopics.StockTicks;
    protected override string ConsumerName => "analytics";

    protected override async Task HandleMessageAsync(TickMessage message, CancellationToken cancellationToken)
    {
        var window = _priceWindows.GetOrAdd(message.Ticker, _ => new List<decimal>());

        decimal? sma = null;
        decimal? ema = null;

        lock (window)
        {
            window.Add(message.Price);
            if (window.Count > WindowSize)
                window.RemoveAt(0);

            if (window.Count >= SmaPeriod)
                sma = TechnicalIndicators.Sma(window, SmaPeriod);

            if (window.Count >= EmaPeriod)
                ema = TechnicalIndicators.Ema(window, EmaPeriod);
        }

        Logger.LogInformation("{Ticker}: price={Price}, SMA20={Sma}, EMA12={Ema}",
            message.Ticker, message.Price, sma, ema);

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var pusher = scope.ServiceProvider.GetService<IQuotePusher>();

            if (pusher is not null)
            {
                await pusher.PushQuoteAsync(
                    message.Ticker,
                    message.Price,
                    sma,
                    ema,
                    message.RecordedAt,
                    cancellationToken);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to push quote update");
        }
    }
}