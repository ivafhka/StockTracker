using Microsoft.EntityFrameworkCore;
using StockTracker.Domain.Entities;
using StockTracker.Domain.Interfaces;
using StockTracker.Domain.ValueObjects;

namespace StockTracker.Infrastructure.Persistence.Repositories
{
    public class AlertRepository : IAlertRepository
    {
        private readonly AppDbContext _dbContext;

        public AlertRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Alert?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Alerts
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Alert>> GetActiveByTickerAsync(Ticker ticker, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Alerts
                .Where(x => x.Ticker == ticker && x.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Alert>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Alerts
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Alert alert, CancellationToken cancellationToken = default)
        {
            await _dbContext.Alerts.AddAsync(alert, cancellationToken);
        }

        public Task UpdateAsync(Alert alert, CancellationToken cancellationToken = default)
        {
            _dbContext.Alerts.Update(alert);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var alert = await _dbContext.Alerts.FindAsync(new object[] { id }, cancellationToken);
            if (alert is not null)
            {
                _dbContext.Alerts.Remove(alert);
            }
        }
    }
}
