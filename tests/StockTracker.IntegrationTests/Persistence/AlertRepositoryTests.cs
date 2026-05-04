
using FluentAssertions;
using StockTracker.Domain.Entities;
using StockTracker.Domain.ValueObjects;
using StockTracker.Infrastructure.Persistence.Repositories;

namespace StockTracker.IntegrationTests.Persistence
{
    [Collection(nameof(PostgresCollection))]
    public class AlertRepositoryTests : PersistenceTestBase
    {
        public AlertRepositoryTests(PostgresFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task GetActiveByTickerAsync_ShouldReturnOnlyActiveAlertsForTicker()
        {
            await CleanupAsync();
            var repository = new AlertRepository(DbContext);
            var userId = Guid.NewGuid();
            var aapl = Ticker.Create("AAPL");
            var msft = Ticker.Create("NSFT");

            var activeAapl1 = Alert.Create(userId, aapl, Money.Create(200m), AlertDirection.Above);
            var activeAapl2 = Alert.Create(userId, aapl, Money.Create(100m), AlertDirection.Below);
            var inactiveAapl = Alert.Create(userId, aapl, Money.Create(150m), AlertDirection.Above);
            inactiveAapl.Deactivate();
            var msftAlert = Alert.Create(userId, msft, Money.Create(300m), AlertDirection.Above);

            await repository.AddAsync(activeAapl1);
            await repository.AddAsync(activeAapl2);
            await repository.AddAsync(inactiveAapl);
            await repository.AddAsync(msftAlert);
            await DbContext.SaveChangesAsync();

            var result = await repository.GetActiveByTickerAsync(aapl);

            result.Should().HaveCount(2);
            result.Should().OnlyContain(x => x.Ticker == aapl && x.IsActive);
        }

        [Fact]
        public async Task UpdateAsync_ShouldPersistDeactivation()
        {
            await CleanupAsync();
            var repository = new AlertRepository(DbContext);
            var alert = Alert.Create(Guid.NewGuid(), Ticker.Create("AAPL"), Money.Create(200m), AlertDirection.Above);
            await repository.AddAsync(alert);
            await DbContext.SaveChangesAsync();
            DbContext.ChangeTracker.Clear();

            var loaded = await repository.GetByIdAsync(alert.Id);
            loaded!.Deactivate();
            await repository.UpdateAsync(loaded);
            await DbContext.SaveChangesAsync();
            DbContext.ChangeTracker.Clear();

            var reloaded = await repository.GetByIdAsync(alert.Id);
            reloaded!.IsActive.Should().BeFalse();
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnAlertsForUser_OrderedByCreatedAtDesc()
        {
            await CleanupAsync();
            var repository = new AlertRepository(DbContext);
            var userId = Guid.NewGuid();

            var alert1 = Alert.Create(userId, Ticker.Create("AAPL"), Money.Create(200m), AlertDirection.Above);
            await Task.Delay(10);
            var alert2 = Alert.Create(userId, Ticker.Create("MSFT"), Money.Create(300m), AlertDirection.Above);
            await Task.Delay(10);
            var alert3 = Alert.Create(userId, Ticker.Create("GOOGL"), Money.Create(2500m), AlertDirection.Above);

            await repository.AddAsync(alert1);
            await repository.AddAsync(alert2);
            await repository.AddAsync(alert3);
            await DbContext.SaveChangesAsync();

            var result = await repository.GetByUserIdAsync(userId);

            result.Should().HaveCount(3);
            result[0].Id.Should().Be(alert3.Id);
            result[2].Id.Should().Be(alert1.Id);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveAlert()
        {
            await CleanupAsync();
            var repository = new AlertRepository(DbContext);
            var alert = Alert.Create(Guid.NewGuid(), Ticker.Create("AAPL"), Money.Create(200m), AlertDirection.Above);
            await repository.AddAsync(alert);
            await DbContext.SaveChangesAsync();

            await repository.DeleteAsync(alert.Id);
            await DbContext.SaveChangesAsync();

            var found = await repository.GetByIdAsync(alert.Id);
            found.Should().BeNull();
        }

        [Fact]
        public async Task EnumDirection_ShouldRoundTrip_Correctly()
        {
            await CleanupAsync();
            var repository = new AlertRepository(DbContext);
            var aboveAlert = Alert.Create(Guid.NewGuid(), Ticker.Create("AAPL"), Money.Create(200m), AlertDirection.Above);
            var belowAlert = Alert.Create(Guid.NewGuid(), Ticker.Create("AAPL"), Money.Create(100m), AlertDirection.Below);

            await repository.AddAsync(aboveAlert);
            await repository.AddAsync(belowAlert);
            await DbContext.SaveChangesAsync();
            DbContext.ChangeTracker.Clear();

            var loadedAbove = await repository.GetByIdAsync(aboveAlert.Id);
            var loadedBelow = await repository.GetByIdAsync(belowAlert.Id);

            loadedAbove!.Direction.Should().Be(AlertDirection.Above);
            loadedBelow!.Direction.Should().Be(AlertDirection.Below);
        }
    }
}
