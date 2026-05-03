using Microsoft.EntityFrameworkCore;
using StockTracker.Domain.Entities;
using StockTracker.Domain.Interfaces;

namespace StockTracker.Infrastructure.Persistence.Repositories
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly AppDbContext _dbContext;

        public PortfolioRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Portfolio?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Portfolios
                .Include(x => x.Positions)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Portfolio>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Portfolios
                .AsNoTracking()
                .Include(x => x.Positions)
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Portfolio portfolio, CancellationToken cancellationToken = default)
        {
            await _dbContext.Portfolios.AddAsync(portfolio, cancellationToken);
        }

        public Task UpdateAsync(Portfolio portfolio, CancellationToken cancellationToken = default)
        {
            var entry = _dbContext.Entry(portfolio);

            if (entry.State == EntityState.Detached)
            {
                _dbContext.Portfolios.Update(portfolio);
            }

            foreach (var position in portfolio.Positions)
            {
                var positionEntry = _dbContext.Entry(position);
                if (positionEntry.State == EntityState.Detached)
                {
                    positionEntry.State = EntityState.Added;
                }
            }

            return Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var portfolio = await _dbContext.Portfolios.FindAsync(new object[] { id }, cancellationToken);
            if (portfolio is not null)
            {
                _dbContext.Portfolios.Remove(portfolio);
            }
        }
    }
}