using StockTracker.Domain.Common;
using StockTracker.Domain.ValueObjects;

namespace StockTracker.Domain.Entities
{
    public class PriceTick : Entity
    {
        public Ticker Ticker { get; private set; } = null!;
        public Money Price { get; private set; } = null!;
        public decimal Volume { get; private set; }
        public DateTime RecordedAt { get; private set; }

        private PriceTick() { }

        public static PriceTick Create(Ticker ticker, Money price, decimal volume, DateTime recordedAt)
        {
            ArgumentNullException.ThrowIfNull(ticker);
            ArgumentNullException.ThrowIfNull(price);

            if (volume < 0)
                throw new ArgumentException("Volume cannot be negative", nameof(volume));

            if (price.Amount <= 0)
                throw new ArgumentException("Price must be positive", nameof(price));

            return new PriceTick
            {
                Ticker = ticker,
                Price = price,
                Volume = volume,
                RecordedAt = recordedAt
            };
        }
    }
}
