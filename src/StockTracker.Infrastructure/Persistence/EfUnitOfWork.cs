
using StockTracker.Application.Interfaces;

namespace StockTracker.Infrastructure.Persistence
{
    public class EfUnitOfWork : IUnitOfWork
    {

        private readonly AppDbContext _dbContext;

        public EfUnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
