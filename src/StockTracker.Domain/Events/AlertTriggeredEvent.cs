using StockTracker.Domain.Common;
using StockTracker.Domain.ValueObjects;

namespace StockTracker.Domain.Events
{
    public record AlertTriggeredEvent
    (
        Guid AlertId,
        Guid UserId,
        Ticker Ticker,
        Money TargetPrice,
        Money CurrentPrice,
        DateTime TriggerAt) : IDomainEvents
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredAt { get; } = DateTime.UtcNow; 
    }
}
