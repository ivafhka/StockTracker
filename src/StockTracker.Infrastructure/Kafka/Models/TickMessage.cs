
namespace StockTracker.Infrastructure.Kafka.Models
{
    public record TickMessage(
        string Ticker,
        decimal Price,
        string Currency,
        decimal Volume,
        DateTime RecordedAt);
}
