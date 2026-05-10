
using Microsoft.EntityFrameworkCore;
using StockTracker.Infrastructure.Persistence;
using Testcontainers.PostgreSql;

namespace StockTracker.IntegrationTests.Persistence
{
    public class PostgresFixture : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithDatabase("stocktracker_tests")
            .WithUsername("postgres")
            .WithPassword("test_password")
            .Build();

        public string ConnectionString => _container.GetConnectionString();

        public async Task InitializeAsync()
        {
            await _container.StartAsync();
            await using var dbContext = CreateDbContext();
            await dbContext.Database.MigrateAsync();
        }

        public async Task DisposeAsync()
        {
            await _container.DisposeAsync();
        }

        public AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(ConnectionString)
                .Options;
            return new AppDbContext(options);
        }
    }
}
