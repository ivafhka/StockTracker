
using Microsoft.Extensions.Options;
using StockTracker.Infrastructure.ExternalApis.Finnhub;
using StockTracker.Infrastructure.Kafka.Models;
using StockTracker.Infrastructure.Kafka.Producer;

namespace StockTracker.Worker
{
    public class QuoteCollectorWorker :BackgroundService
    {
        private readonly FinnhubWebSocketClient _finnhubClient;
        private readonly ITickProducer _tickProducer;
        private readonly FinnhubOptions _options;
        private readonly ILogger<QuoteCollectorWorker> _logger;

        public QuoteCollectorWorker(
            FinnhubWebSocketClient finnhubClient,
            ITickProducer tickProducer,
            IOptions<FinnhubOptions> options,
            ILogger<QuoteCollectorWorker> logger)
        {
            _finnhubClient = finnhubClient;
            _tickProducer = tickProducer;
            _options = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Qoute Collector worker started");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await RunCollectionLoopAsync(cancellationToken);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Worker is stopping");
                    break;
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Error in collection loop. Reconnectiong in {DelayMs}ms", _options.ReconnectDelayMs);
                    await Task.Delay(_options.ReconnectDelayMs, cancellationToken);
                }
            }
        }

        private async Task RunCollectionLoopAsync(CancellationToken cancellationToken)
        {
            await _finnhubClient.ConnectAsync(cancellationToken);

            await foreach (var tick in _finnhubClient.ReciveTicksAsync(cancellationToken))
            {
                var message = new TickMessage(
                    Ticker: tick.Ticker,
                    Price: tick.Price,
                    Currency: "USD",
                    Volume: tick.Volume,
                    RecordedAt: tick.RecordedAt);

                try
                {
                    await _tickProducer.ProduceAsync(message, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to produce tick for {Ticker}", tick.Ticker);
                }
            }
        }
    }
}
