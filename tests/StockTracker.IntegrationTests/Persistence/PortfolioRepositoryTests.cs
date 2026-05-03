
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StockTracker.Domain.Entities;
using StockTracker.Domain.ValueObjects;
using StockTracker.Infrastructure.Persistence.Repositories;

namespace StockTracker.IntegrationTests.Persistence
{
    [Collection(nameof(PostgresCollection))]
    public class PortfolioRepositoryTests : PersistenceTestBase
    {
        public PortfolioRepositoryTests(PostgresFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task AddAsync_ShouldPersistPortfolio()
        {
            await CleanupAsync();
            var repository = new PortfolioRepository(DbContext);
            var portfolio = Portfolio.Create(Guid.NewGuid(), "My Portfolio", "Description");

            await repository.AddAsync(portfolio);
            await DbContext.SaveChangesAsync();

            var saved = await repository.GetByIdAsync(portfolio.Id);
            saved.Should().NotBeNull();
            saved!.Name.Should().Be("My Portfolio");
            saved.Description.Should().Be("Description");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldLoadPositions_AlongWithPortfolio()
        {
            await CleanupAsync();
            var repository = new PortfolioRepository(DbContext);
            var portfolio = Portfolio.Create(Guid.NewGuid(), "Test");
            portfolio.AddPosition(Ticker.Create("AAPL"), 10, Money.Create(150m));
            portfolio.AddPosition(Ticker.Create("MSFT"), 5, Money.Create(300m));

            await repository.AddAsync(portfolio);
            await DbContext.SaveChangesAsync();
            DbContext.ChangeTracker.Clear();

            var loaded = await repository.GetByIdAsync(portfolio.Id);

            loaded.Should().NotBeNull();
            loaded!.Positions.Should().HaveCount(2);
            loaded.Positions.Select(x => x.Ticker.Symbol).Should().Contain(new[] { "AAPL", "MSFT" });
        }

        [Fact]
        public async Task ValueObject_ShouldRoundTrip_Correctly()
        {
            await CleanupAsync();
            var repository = new PortfolioRepository(DbContext);
            var portfolio = Portfolio.Create(Guid.NewGuid(), "Test");
            var ticker = Ticker.Create("AAPL");
            var price = Money.Create(150.75m, "USD");
            portfolio.AddPosition(ticker, 10, price);

            await repository.AddAsync(portfolio);
            await DbContext.SaveChangesAsync();
            DbContext.ChangeTracker.Clear();

            var loaded = await repository.GetByIdAsync(portfolio.Id);
            var position = loaded!.Positions.Single();

            position.Ticker.Should().Be(ticker);
            position.AvgBuyPrice.Should().Be(price);
            position.AvgBuyPrice.Amount.Should().Be(150.75m);
            position.AvgBuyPrice.Currency.Should().Be("USD");
        }

        [Fact]
        public async Task GetUserIdAsync_ShouldReturnOnlyUserPortfolios()
        {
            await CleanupAsync();
            var repository = new PortfolioRepository(DbContext);
            var user1Id = Guid.NewGuid();
            var user2Id = Guid.NewGuid();

            await repository.AddAsync(Portfolio.Create(user1Id, "User1 Portfolio 1"));
            await repository.AddAsync(Portfolio.Create(user1Id, "User1 Portfolio 2"));
            await repository.AddAsync(Portfolio.Create(user2Id, "User2 Portfolio"));
            await DbContext.SaveChangesAsync();

            var user1Portfolios = await repository.GetByUserIdAsync(user1Id);

            user1Portfolios.Should().HaveCount(2);
            user1Portfolios.Should().OnlyContain(x=>x.UserId == user1Id);
        }

        [Fact]
        public async Task DeletePortfolio_ShouldCascadeToPositions()
        {
            await CleanupAsync();
            var repository = new PortfolioRepository(DbContext);
            var portolio = Portfolio.Create(Guid.NewGuid(), "Test");
            portolio.AddPosition(Ticker.Create("AAPL"), 10, Money.Create(150m));
            portolio.AddPosition(Ticker.Create("MSFT"), 5, Money.Create(300m));

            await repository.AddAsync(portolio);
            await DbContext.SaveChangesAsync();

            await repository.DeleteAsync(portolio.Id);
            await DbContext.SaveChangesAsync();

            var remaining = await DbContext.Positions.CountAsync();
            remaining.Should().Be(0);
        }

        [Fact(Skip = "Inverstigating EF Core change tracking issue")]
        public async Task UpdateAsync_ShouldPersistChanges()
        {
            await CleanupAsync();
            var repository = new PortfolioRepository(DbContext);
            var portfolio = Portfolio.Create(Guid.NewGuid(), "Test");
            await repository.AddAsync(portfolio);
            await DbContext.SaveChangesAsync();
            DbContext.ChangeTracker.Clear();

            var loaded =await repository.GetByIdAsync(portfolio.Id);
            loaded!.AddPosition(Ticker.Create("GOOGL"), 3, Money.Create(2500m));
            await repository.UpdateAsync(loaded);
            await DbContext.SaveChangesAsync();
            DbContext.ChangeTracker.Clear();

            var reloaded = await repository.GetByIdAsync(portfolio.Id);
            reloaded!.Positions.Should().HaveCount(1);
            reloaded.Positions.First().Ticker.Symbol.Should().Be("GOOGL");
        }
    }
}
