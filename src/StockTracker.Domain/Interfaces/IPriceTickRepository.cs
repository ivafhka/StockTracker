using StockTracker.Domain.Entities;
using StockTracker.Domain.ValueObjects;

namespace StockTracker.Domain.Interfaces
{
    public interface IPriceTickRepository
    {
        Task<IReadOnlyList<PriceTick>> GetHistoryAsync(
            Ticker ticker, 
            DateTime from,
            DateTime to,
            CancellationToken cancellationToken = default);

        Task<PriceTick?> GetLatestAsync(Ticker ticker, CancellationToken cancellationToken = default);
        Task AddBatchAsync(IEnumerable<PriceTick> ticks, CancellationToken cancellationToken = default);
    }
}
