
namespace StockTracker.Domain.Common
{
    public interface IDomainEvents
    {
        Guid EventId { get; }
        DateTime OccurredAt { get; }
    }
}
