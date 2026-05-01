using Microsoft.EntityFrameworkCore;
using StockTracker.Domain.Entities;
using StockTracker.Domain.Interfaces;
using StockTracker.Domain.ValueObjects;

namespace StockTracker.Infrastructure.Persistence.Repositories
{
    public class PriceTickRepository : IPriceTickRepository
    {
        private readonly AppDbContext _dbContext;

        public PriceTickRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<PriceTick>> GetHistoryAsync(Ticker ticker, DateTime from, DateTime to, CancellationToken cancellationToken = default)
        {
            return await _dbContext.PriceTicks
                .Where(x => x.Ticker == ticker && x.RecordedAt >= from && x.RecordedAt <= to)
                .OrderBy(x => x.RecordedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<PriceTick?> GetLatestAsync(Ticker ticker, CancellationToken cancellationToken = default)
        {
            return await _dbContext.PriceTicks
                .Where(x => x.Ticker == ticker)
                .OrderByDescending(x => x.RecordedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddBatchAsync(IEnumerable<PriceTick> ticks, CancellationToken cancellationToken = default)
        {
            await _dbContext.PriceTicks.AddRangeAsync(ticks, cancellationToken);
        }
    }
}
