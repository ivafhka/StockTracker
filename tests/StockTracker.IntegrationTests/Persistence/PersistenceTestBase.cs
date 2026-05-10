
using Microsoft.EntityFrameworkCore;
using StockTracker.Infrastructure.Persistence;

namespace StockTracker.IntegrationTests.Persistence
{
    public abstract class PersistenceTestBase : IAsyncLifetime
    {
        protected readonly PostgresFixture Fixture;
        protected AppDbContext DbContext = null!;

        protected PersistenceTestBase(PostgresFixture fixture)
        {
            Fixture = fixture;
        }

        public Task InitializeAsync()
        {
            DbContext = Fixture.CreateDbContext();
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await DbContext.DisposeAsync();
        }

        protected async Task CleanupAsync()
        {
            await DbContext.Database.ExecuteSqlRawAsync(
                "TRUNCATE TABLE alerts, positions, portfolios, users, price_ticks RESTART IDENTITY CASCADE");
        }
    }
}
