using StockTracker.Domain.Common;
using StockTracker.Domain.ValueObjects;

namespace StockTracker.Domain.Entities
{
    public class Position : Entity
    {
        public Guid PortfolioId { get; private set; }
        public Ticker Ticker { get; private set; } = null!;
        public decimal Quanity { get; private set; }
        public Money AvgBuyPrice { get; private set; } = null!;
        public DateTime OpenedAt { get; private set; }

        private Position () { }

        internal static Position Open(Guid portfolioId, Ticker ticker, decimal quanity, Money buyprice)
        {
            ArgumentNullException.ThrowIfNull(ticker);
            ArgumentNullException.ThrowIfNull(buyprice);

            if (quanity <= 0)
                throw new ArgumentException("Quanity must be positive", nameof(quanity));

            return new Position
            {
                PortfolioId = portfolioId,
                Ticker = ticker,
                Quanity = quanity,
                AvgBuyPrice = buyprice,
                OpenedAt = DateTime.UtcNow
            };
        }

        public void IncreaseQuanity(decimal additionalQuanity, Money buyPrice)
        {
            ArgumentNullException.ThrowIfNull(buyPrice);

            if (additionalQuanity <= 0)
                throw new ArgumentException("Additional quanity must be positive", nameof(additionalQuanity));

            var totalCost = AvgBuyPrice.Multiply(Quanity).Add(buyPrice.Multiply(additionalQuanity));
            var newQuanity = Quanity + additionalQuanity;

            Quanity = newQuanity;
            AvgBuyPrice = Money.Create(totalCost.Amount / newQuanity, buyPrice.Currency);
        }

        public Money CalculateCurrntValue(Money currnetPrice)
        {
            ArgumentNullException.ThrowIfNull(currnetPrice);
            return currnetPrice.Multiply(Quanity);
        }

        public Money CalculatePnL(Money currentPrice)
        {
            ArgumentNullException.ThrowIfNull(currentPrice);
            var currnetvalue = currentPrice.Multiply(Quanity);
            var costBasis = AvgBuyPrice.Multiply(Quanity);
            return currnetvalue.Subtract(costBasis);
        }
    }
}
