using StockTracker.Domain.Common;
using StockTracker.Domain.ValueObjects;

namespace StockTracker.Domain.Events
{
    public record PositionOpenedEvent
    (
        Guid PositionId,
        Guid PortfolioId,
        Ticker Ticker,
        decimal Quanity,
        Money BuyPrice ) : IDomainEvents
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredAt { get; } = DateTime.UtcNow;
    }
}
