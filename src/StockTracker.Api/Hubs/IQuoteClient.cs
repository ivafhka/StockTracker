namespace StockTracker.Api.Hubs
{
    public interface IQuoteClient
    {
        Task ReceiveQuote(QuoteUpdate update);
        Task ReceiveAlert(AlertUpdate update);
    }

    public record QuoteUpdate(
        string Ticker,
        decimal Price,
        decimal? Sma20,
        decimal? Ema12,
        DateTime Timestamp);

    public record AlertUpdate(
        Guid AlertId,
        string Ticker,
        decimal TargetPrice,
        decimal CurrentPrice,
        string Direction,
        DateTime TriggeredAt);
}
