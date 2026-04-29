namespace StockTracker.Application.Queries.GetPriceHistory
{
    public record PriceTickDto(
        string Ticker,
        decimal Price,
        string Currency,
        decimal Volume,
        DateTime RecordedAt);
}
