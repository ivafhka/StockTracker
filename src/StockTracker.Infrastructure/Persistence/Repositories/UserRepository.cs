using Microsoft.EntityFrameworkCore;
using StockTracker.Domain.Entities;
using StockTracker.Domain.Interfaces;

namespace StockTracker.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var normalized = email.ToLowerInvariant();
            return await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Email == normalized, cancellationToken);
        }

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var normalized = email.ToLowerInvariant();
            return await _dbContext.Users
                .AnyAsync(x => x.Email == email, cancellationToken);
        }
        public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            await _dbContext.Users.AddAsync(user, cancellationToken);
        }
    }
}
