using StockTracker.Domain.Entities;
using StockTracker.Domain.ValueObjects;

namespace StockTracker.Application.Services
{
    public class PortfolioValuationService : IPortfolioValuationService
    {
        public PortfolioValuation Calculate(
            Portfolio portfolio,
            IReadOnlyDictionary<Ticker, Money> currentPrices)
        {
            ArgumentNullException.ThrowIfNull(portfolio);
            ArgumentNullException.ThrowIfNull(currentPrices);

            if(portfolio.Positions.Count == 0)
            {
                return new PortfolioValuation(
                    TotalCost: 0,
                    CurrentValue: 0,
                    TotalPnL: 0,
                    PnlPercentage: 0,
                    Currency: "USD",
                    Positions: Array.Empty<PositionValuation>());
            }

            var positionValuations = new List<PositionValuation>();
            decimal totalCost = 0;
            decimal totalCurrentValue = 0;
            var currency = portfolio.Positions.First().AvgBuyPrice.Currency;

            foreach( var position in portfolio.Positions)
            {
                if(!currentPrices.TryGetValue(position.Ticker, out var currentPrice))
                {
                    currentPrice = position.AvgBuyPrice;
                }

                var costBasis = position.AvgBuyPrice.Amount * position.quantity;
                var currentValue = currentPrice.Amount * position.quantity;
                var pnl = currentValue - costBasis;
                var pnlPercentage = costBasis == 0 ? 0 : (pnl / costBasis) * 100;

                positionValuations.Add(new PositionValuation(
                    PositionId: position.Id,
                    Ticker: position.Ticker.Symbol,
                    Quantity: position.quantity,
                    AvgBuyPrice: position.AvgBuyPrice.Amount,
                    CurrentPrice: currentPrice.Amount,
                    CurrentValue: currentValue,
                    PnL: pnl,
                    PnlPercentage: Math.Round(pnlPercentage,2)));

                totalCost += costBasis;
                totalCurrentValue += currentValue;
            }

            var totalPnL = totalCurrentValue - totalCost;
            var totalPnLPercentage = totalCost == 0 ? 0 : (totalPnL / totalCost) * 100;

            return new PortfolioValuation(
                TotalCost: totalCost,
                CurrentValue: totalCurrentValue,
                TotalPnL: totalPnL,
                PnlPercentage: Math.Round(totalPnLPercentage,2),
                Currency: currency,
                Positions: positionValuations);
        }
    }
}
