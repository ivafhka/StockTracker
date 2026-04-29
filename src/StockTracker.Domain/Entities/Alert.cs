using StockTracker.Domain.Common;
using StockTracker.Domain.ValueObjects;

namespace StockTracker.Domain.Entities
{
    public enum AlertDirection
    {
        Above = 1, 
        Below = 2
    }
    public class Alert :Entity
    {
        public Guid UserId { get; private set; }
        public Ticker Ticker { get; private set; } = null!;
        public Money TargetPrice { get; private set; } = null!;
        public AlertDirection Direction { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Alert () { }

        public static Alert Create(Guid userId, Ticker ticker, Money targetPrice, AlertDirection direction)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User id is required", nameof(userId));

            ArgumentNullException.ThrowIfNull(ticker);
            ArgumentNullException.ThrowIfNull(targetPrice);

            if (targetPrice.Amount <= 0)
                throw new ArgumentException("Target price must be positive", nameof(targetPrice));

            return new Alert
            {
                UserId = userId,
                Ticker = ticker,
                TargetPrice = targetPrice,
                Direction = direction,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        public bool ShouldTrigger(Money currentPrice)
        {
            ArgumentNullException.ThrowIfNull(currentPrice);

            if (!IsActive) return false;

            return Direction switch
            {
                AlertDirection.Above => currentPrice.Amount >= TargetPrice.Amount,
                AlertDirection.Below => currentPrice.Amount <= TargetPrice.Amount,
                _ => false
            };
        }

        public void Deactivate() => IsActive = false;

        public void Activate() => IsActive = true;
    }
}
