using Microsoft.EntityFrameworkCore;
using StockTracker.Domain.Entities;

namespace StockTracker.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Portfolio> Portfolios => Set<Portfolio>();
        public DbSet<Position> Positions => Set<Position>();
        public DbSet<Alert> Alerts => Set<Alert>();
        public DbSet<PriceTick> PriceTicks => Set<PriceTick>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
