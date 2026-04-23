using StockTracker.Domain.Entities;

namespace StockTracker.Domain.Interfaces
{
    public interface IPortfolioRepository
    {
        Task<Portfolio?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Portfolio>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task AddAsync(Portfolio portfolio, CancellationToken cancellationToken = default);
        Task UpdateAsync(Portfolio portfolio, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
