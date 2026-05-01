using StockTracker.Domain.Entities;
using StockTracker.Domain.ValueObjects;

namespace StockTracker.Application.Queries.GetAlerts
{
    public record AlertDto(
        Guid Id,
        string Ticker,
        decimal TargetPrice,
        string Currency,
        AlertDirection Direction,
        bool IsActive,
        DateTime CreatedAt);
}
