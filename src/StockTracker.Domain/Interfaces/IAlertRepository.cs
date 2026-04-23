using StockTracker.Domain.Entities;
using StockTracker.Domain.ValueObjects;

namespace StockTracker.Domain.Interfaces
{
    public interface IAlertRepository
    {
        Task<Alert?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Alert>> GetActiveByTickerAsync(Ticker ticker, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Alert>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task AddAsync(Alert alert, CancellationToken cancellationToken = default);
        Task UpdateAsync(Alert alert, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
