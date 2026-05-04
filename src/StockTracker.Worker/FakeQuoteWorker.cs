using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StockTracker.Infrastructure.Kafka.Models;
using StockTracker.Infrastructure.Kafka.Producer;

namespace StockTracker.Worker;

public class FakeQuoteWorker : BackgroundService
{
    private readonly ITickProducer _tickProducer;
    private readonly ILogger<FakeQuoteWorker> _logger;
    private readonly Random _random = new();
    private readonly string[] _tickers = { "AAPL", "MSFT", "GOOGL", "TSLA" };

    public FakeQuoteWorker(ITickProducer tickProducer, ILogger<FakeQuoteWorker> logger)
    {
        _tickProducer = tickProducer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Fake quote worker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            var ticker = _tickers[_random.Next(_tickers.Length)];
            var price = (decimal)(100 + _random.NextDouble() * 200);

            var message = new TickMessage(
                Ticker: ticker,
                Price: Math.Round(price, 2),
                Currency: "USD",
                Volume: _random.Next(100, 10000),
                RecordedAt: DateTime.UtcNow);

            try
            {
                await _tickProducer.ProduceAsync(message, stoppingToken);
                _logger.LogDebug("Sent fake tick {Ticker} {Price}", ticker, message.Price);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send fake tick");
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}