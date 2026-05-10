using StockTracker.Domain.Common;
using StockTracker.Domain.ValueObjects;

namespace StockTracker.Domain.Entities
{
    public class Portfolio : Entity
    {
        private readonly List<Position> _positions = new();

        public Guid UserId { get; private set; }
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public IReadOnlyList<Position> Positions => _positions.AsReadOnly();

        private Portfolio () { }

        public static Portfolio Create(Guid userId, string name, string? descripion = null)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User id is required", nameof(userId));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Portfolio anme is required", nameof(name));

            return new Portfolio
            {
                UserId = userId,
                Name = name,
                Description = descripion,
                CreatedAt = DateTime.UtcNow
            };
        }

        public Position AddPosition(Ticker ticker, decimal quantity, Money buyPrice)
        {
            ArgumentNullException.ThrowIfNull(ticker);

            var existing = _positions.FirstOrDefault(x => x.Ticker == ticker);

            if(existing is not null)
            {
                existing.Increasequantity(quantity, buyPrice);
                return existing;
            }

            var position = Position.Open(Id, ticker, quantity, buyPrice);
            _positions.Add(position);
            return position;
        }

        public void RemovePosition(Guid positionId)
        {
            var position = _positions.FirstOrDefault(x => x.Id == positionId);

            if (position is null)
                throw new ArgumentException($"Position {positionId} new found in portfolio");

            _positions.Remove(position);
        }
    }
}
