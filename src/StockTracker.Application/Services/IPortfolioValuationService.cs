using StockTracker.Domain.Entities;
using StockTracker.Domain.ValueObjects;
using System.Text;

namespace StockTracker.Application.Services
{
    public interface IPortfolioValuationService 
    {
        PortfolioValuation Calculate(
            Portfolio portfolio,
            IReadOnlyDictionary<Ticker, Money> currentPrices);
    }

    public record PortfolioValuation(
        decimal TotalCost,
        decimal CurrentValue,
        decimal TotalPnL,
        decimal PnlPercentage,
        string Currency,
        IReadOnlyList<PositionValuation> Positions);

    public record PositionValuation(
        Guid PositionId,
        string Ticker,
        decimal Quantity,
        decimal AvgBuyPrice,
        decimal CurrentPrice,
        decimal CurrentValue,
        decimal PnL,
        decimal PnlPercentage);
}
