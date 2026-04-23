using FluentAssertions;
using StockTracker.Domain.Services;

namespace StockTracker.UnitTests.Domain.Services
{
    public class TechnicalIndicatorsTests
    {
        [Fact]
        public void Sma_ShouldCalculateCorrectly_ForSimpleCase()
        {
            var prices = new decimal[] { 10m, 11m, 12m, 13m, 14m };

            var sma = TechnicalIndicators.Sma(prices, period: 3);

            sma.Should().Be(13m);
        }

        [Fact]
        public void Sma_ShouldUseLastPrices()
        {
            var prices = new decimal[] { 100m, 200m, 10m, 20m, 30m };

            var sma = TechnicalIndicators.Sma(prices, period: 3);

            sma.Should().Be(20m);
        }

        [Fact]
        public void Sma_ShouldEqualOnlyValue_WhenPeriodEqualsOne()
        {
            var prices = new decimal[] { 5m, 10m, 15m };

            var sma = TechnicalIndicators.Sma(prices, period: 1);

            sma.Should().Be(15m);
        }

        [Fact]
        public void Sma_ShouldThrow_WhenPricesEmpty()
        {
            var act = () => TechnicalIndicators.Sma(Array.Empty<decimal>(), period: 3);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Sma_ShouldThrow_WhenPeriodGreaterthanPricesCount()
        {
            var prices = new decimal[] { 10m, 11m };

            var act = () => TechnicalIndicators.Sma(prices, period: 5);

            act.Should().Throw<ArgumentException>()
                .WithMessage("*Not enough data*");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Sma_ShouldThrow_WhenPeriodIsNotPositive(int period)
        {
            var prices = new decimal[] { 10m, 11m, 12m };

            var act = () => TechnicalIndicators.Sma(prices, period);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Ema_ShouldCalculateCorrectly_ForKnownSequence()
        {
            var prices = new decimal[] { 10m, 11m, 12m, 13m, 14m, 15m };

            var ema = TechnicalIndicators.Ema(prices, period: 3);

            ema.Should().BeApproximately(14.0m, 0.1m);
        }

        [Fact]
        public void Ema_ShouldReactFasterToNewData_ThanSma()
        {
            var prices = new decimal[] { 10m, 10m, 10m, 10m, 10m, 20m };

            var sma = TechnicalIndicators.Sma(prices, period: 5);
            var ema = TechnicalIndicators.Ema(prices, period: 5);

            ema.Should().BeGreaterThan(sma);
        }

        [Fact]
        public void Ema_ShouldThrow_WhenPricesEmpty()
        {
            var act = () => TechnicalIndicators.Ema(Array.Empty<decimal>(), period: 3);

            act.Should().Throw<ArgumentException>();
        }
    }
}
