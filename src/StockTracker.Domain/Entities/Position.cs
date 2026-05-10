using StockTracker.Domain.Common;
using StockTracker.Domain.ValueObjects;

namespace StockTracker.Domain.Entities
{
    public class Position : Entity
    {
        public Guid PortfolioId { get; private set; }
        public Ticker Ticker { get; private set; } = null!;
        public decimal quantity { get; private set; }
        public Money AvgBuyPrice { get; private set; } = null!;
        public DateTime OpenedAt { get; private set; }

        private Position () { }

        internal static Position Open(Guid portfolioId, Ticker ticker, decimal quantity, Money buyprice)
        {
            ArgumentNullException.ThrowIfNull(ticker);
            ArgumentNullException.ThrowIfNull(buyprice);

            if (quantity <= 0)
                throw new ArgumentException("quantity must be positive", nameof(quantity));

            return new Position
            {
                PortfolioId = portfolioId,
                Ticker = ticker,
                quantity = quantity,
                AvgBuyPrice = buyprice,
                OpenedAt = DateTime.UtcNow
            };
        }

        public void Increasequantity(decimal additionalquantity, Money buyPrice)
        {
            ArgumentNullException.ThrowIfNull(buyPrice);

            if (additionalquantity <= 0)
                throw new ArgumentException("Additional quantity must be positive", nameof(additionalquantity));

            var totalCost = AvgBuyPrice.Multiply(quantity).Add(buyPrice.Multiply(additionalquantity));
            var newquantity = quantity + additionalquantity;

            quantity = newquantity;
            AvgBuyPrice = Money.Create(totalCost.Amount / newquantity, buyPrice.Currency);
        }

        public Money CalculateCurrntValue(Money currnetPrice)
        {
            ArgumentNullException.ThrowIfNull(currnetPrice);
            return currnetPrice.Multiply(quantity);
        }

        public Money CalculatePnL(Money currentPrice)
        {
            ArgumentNullException.ThrowIfNull(currentPrice);
            var currnetvalue = currentPrice.Multiply(quantity);
            var costBasis = AvgBuyPrice.Multiply(quantity);
            return currnetvalue.Subtract(costBasis);
        }
    }
}
