using FluentAssertions;
using StockTracker.Domain.Entities;
using StockTracker.Domain.ValueObjects;

namespace StockTracker.UnitTests.Domain.Entities
{
    public class AlertTests
    {
        private static readonly Guid TestUserId = Guid.NewGuid();

        [Fact]
        public void Create_ShouldSucceed_WithValidData()
        {
            var alert = Alert.Create(
                userId: TestUserId,
                ticker: Ticker.Create("AAPL"),
                targetPrice: Money.Create(200m),
                direction: AlertDirection.Above);

            alert.UserId.Should().Be(TestUserId);
            alert.Ticker.Symbol.Should().Be("AAPL");
            alert.TargetPrice.Amount.Should().Be(200m);
            alert.Direction.Should().Be(AlertDirection.Above);
            alert.IsActive.Should().BeTrue();
        }

        [Fact]
        public void Create_ShouldThrow_WhenUserIdIsEmpty()
        {
            var act = () => Alert.Create(
                userId: Guid.Empty,
                ticker: Ticker.Create("AAPL"),
                targetPrice: Money.Create(200m),
                direction: AlertDirection.Above);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Create_ShouldThrow_WhenTargetPriceIsZero()
        {
            var act = () => Alert.Create(
                userId: TestUserId,
                ticker: Ticker.Create("AAPL"),
                targetPrice: Money.Create(0m),
                direction: AlertDirection.Above);

            act.Should().Throw<ArgumentException>().
                WithMessage("*must be positive*");
        }

        [Theory]
        [InlineData(199.99, false)]
        [InlineData(200.00, true)]
        [InlineData(250.00, true)]
        public void ShouldTrigger_ForAboveDirection_ReturnsCorrectResult(decimal currentPrice, bool expected)
        {
            var alert = Alert.Create(TestUserId, Ticker.Create("AAPL"), Money.Create(200m), AlertDirection.Above);

            var result = alert.ShouldTrigger(Money.Create(currentPrice));

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(100.01, false)]
        [InlineData(100.00, true)]
        [InlineData(80.00, true)]
        public void ShouldTrigger_ForBelowDirection_ReturnsCorrectResult(decimal currentPrice, bool expected)
        {
            var alert = Alert.Create(TestUserId, Ticker.Create("AAPL"), Money.Create(100m), AlertDirection.Below);

            var result = alert.ShouldTrigger(Money.Create(currentPrice));

            result.Should().Be(expected);
        }

        [Fact]
        public void ShouldTrigger_ShouldReturnFalse_WhenAlertDeactivated()
        {
            var alert = Alert.Create(TestUserId, Ticker.Create("AAPL"), Money.Create(200m), AlertDirection.Above);
            alert.Deactivate();

            var result = alert.ShouldTrigger(Money.Create(250m));

            result.Should().BeFalse();
        }

        [Fact]
        public void Activate_ShouldSetIsActiveTrue()
        {
            var alert = Alert.Create(TestUserId, Ticker.Create("AAPL"), Money.Create(200m), AlertDirection.Above);
            alert.Deactivate();

            alert.Activate();

            alert.IsActive.Should().BeTrue();
        }

        [Fact]
        public void Deactivate_ShouldSetIsActiveFalse()
        {
            var alert = Alert.Create(TestUserId, Ticker.Create("AAPL"), Money.Create(200m), AlertDirection.Above);

            alert.Deactivate();

            alert.IsActive.Should().BeFalse();
        }
    }
}
