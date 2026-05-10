using FluentAssertions;
using StockTracker.Domain.Entities;
using StockTracker.Domain.ValueObjects;
using StockTracker.Infrastructure.Persistence.Repositories;

namespace StockTracker.IntegrationTests.Persistence;

[Collection(nameof(PostgresCollection))]
public class PriceTickRepositoryTests : PersistenceTestBase
{
    public PriceTickRepositoryTests(PostgresFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task AddBatchAsync_ShouldPersistAllTicks()
    {
        await CleanupAsync();
        var repository = new PriceTickRepository(DbContext);
        var ticker = Ticker.Create("AAPL");
        var ticks = Enumerable.Range(0, 100)
            .Select(i => PriceTick.Create(
                ticker,
                Money.Create(150m + i),
                volume: 1000,
                recordedAt: DateTime.UtcNow.AddMinutes(-i)))
            .ToList();

        await repository.AddBatchAsync(ticks);
        await DbContext.SaveChangesAsync();

        var history = await repository.GetHistoryAsync(
            ticker,
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddDays(1));

        history.Should().HaveCount(100);
    }

    [Fact]
    public async Task GetHistoryAsync_ShouldReturnTicksInDateRange_OrderedByTime()
    {
        await CleanupAsync();
        var repository = new PriceTickRepository(DbContext);
        var ticker = Ticker.Create("AAPL");
        var baseTime = new DateTime(2026, 1, 15, 10, 0, 0, DateTimeKind.Utc);

        var ticks = new[]
        {
            PriceTick.Create(ticker, Money.Create(100m), 100, baseTime.AddMinutes(0)),
            PriceTick.Create(ticker, Money.Create(101m), 100, baseTime.AddMinutes(5)),
            PriceTick.Create(ticker, Money.Create(102m), 100, baseTime.AddMinutes(10)),
            PriceTick.Create(ticker, Money.Create(99m),  100, baseTime.AddMinutes(-30)),
            PriceTick.Create(ticker, Money.Create(105m), 100, baseTime.AddHours(2))
        };

        await repository.AddBatchAsync(ticks);
        await DbContext.SaveChangesAsync();

        var history = await repository.GetHistoryAsync(
            ticker,
            baseTime,
            baseTime.AddMinutes(15));

        history.Should().HaveCount(3);
        history.Select(t => t.Price.Amount).Should().Equal(100m, 101m, 102m);
    }

    [Fact]
    public async Task GetLatestAsync_ShouldReturnMostRecentTick()
    {
        await CleanupAsync();
        var repository = new PriceTickRepository(DbContext);
        var ticker = Ticker.Create("AAPL");
        var baseTime = DateTime.UtcNow;

        await repository.AddBatchAsync(new[]
        {
            PriceTick.Create(ticker, Money.Create(100m), 100, baseTime.AddMinutes(-30)),
            PriceTick.Create(ticker, Money.Create(105m), 100, baseTime.AddMinutes(-5)),
            PriceTick.Create(ticker, Money.Create(102m), 100, baseTime.AddMinutes(-15))
        });
        await DbContext.SaveChangesAsync();

        var latest = await repository.GetLatestAsync(ticker);

        latest.Should().NotBeNull();
        latest!.Price.Amount.Should().Be(105m);
    }

    [Fact]
    public async Task GetHistoryAsync_ShouldReturnEmpty_whenNoTicksInRange()
    {
        await CleanupAsync();
        var repository = new PriceTickRepository(DbContext);
        var ticker = Ticker.Create("AAPL");

        var history = await repository.GetHistoryAsync(
            ticker,
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow);

        history.Should().BeEmpty();
    }
}