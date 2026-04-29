using FluentAssertions;
using StockTracker.Application.Services;
using StockTracker.Domain.Entities;
using StockTracker.Domain.ValueObjects;

namespace StockTracker.UnitTests.Application.Services
{
    public class PortfolioValuationServiceTests
    {
        private readonly PortfolioValuationService _service = new();

        [Fact]
        public void Calculate_ShouldReturnZero_WhenPortfolioHasNoPositions()
        {
            var portfolio = Portfolio.Create(Guid.NewGuid(), "Empty");
            var prices = new Dictionary<Ticker, Money>();

            var result = _service.Calculate(portfolio, prices);

            result.TotalCost.Should().Be(0);
            result.CurrentValue.Should().Be(0);
            result.TotalPnL.Should().Be(0);
            result.Positions.Should().BeEmpty();
        }

        [Fact]
        public void Calculate_ShouldComputeProfit_WhenPriceWnetUp()
        {
            var portfolio = Portfolio.Create(Guid.NewGuid(), "Test");
            var ticker = Ticker.Create("AAPL");
            portfolio.AddPosition(ticker, 10, Money.Create(100m));

            var currentPrice = new Dictionary<Ticker, Money>
            {
                [ticker] = Money.Create(150m)
            };

            var result = _service.Calculate(portfolio, currentPrice);

            result.TotalCost.Should().Be(1000m);
            result.CurrentValue.Should().Be(1500m);
            result.TotalPnL.Should().Be(500m);
            result.PnlPercentage.Should().Be(50m);
        }

        [Fact]
        public void Calculate_ShouldComputeLoss_WhenPriceWentDown()
        {
            var portfolio = Portfolio.Create(Guid.NewGuid(), "Test");
            var ticker = Ticker.Create("AAPL");
            portfolio.AddPosition(ticker, 10, Money.Create(100m));

            var currentPrices = new Dictionary<Ticker, Money>
            {
                [ticker] = Money.Create(80m)
            };

            var result = _service.Calculate(portfolio, currentPrices);

            result.TotalPnL.Should().Be(-200m);
            result.PnlPercentage.Should().Be(-20m);
        }

        [Fact]
        public void Calculate_ShouldUseAvgBuyPrice_WhenCurrentPriceMissimg()
        {
            var portfolio = Portfolio.Create(Guid.NewGuid(), "Test");
            var ticker = Ticker.Create("AAPL");
            portfolio.AddPosition(ticker, 10, Money.Create(100m));

            var emptyPrices = new Dictionary<Ticker, Money>();

            var result = _service.Calculate(portfolio, emptyPrices);

            result.TotalPnL.Should().Be(0);
            result.PnlPercentage.Should().Be(0);
        }

        [Fact]
        public void Calculate_ShouldHandleMultiplePositions()
        {
            var portfolio = Portfolio.Create(Guid.NewGuid(), "Test");
            var aapl = Ticker.Create("AAPL");
            var msft = Ticker.Create("MSFT");

            portfolio.AddPosition(aapl, 10, Money.Create(100m));
            portfolio.AddPosition(msft, 5, Money.Create(200m));

            var currentPrices = new Dictionary<Ticker, Money>
            {
                [aapl] = Money.Create(120m),
                [msft] = Money.Create(180m)
            };

            var result = _service.Calculate(portfolio, currentPrices);

            result.TotalCost.Should().Be(2000m);
            result.CurrentValue.Should().Be(2100m);
            result.TotalPnL.Should().Be(100m);
            result.Positions.Should().HaveCount(2);
        }
    }
}
