namespace StockTracker.Application.Interfaces;

public interface IQuotePusher
{
    Task PushQuoteAsync(
        string ticker,
        decimal price,
        decimal? sma20,
        decimal? ema12,
        DateTime timestamp,
        CancellationToken cancellationToken = default);
}