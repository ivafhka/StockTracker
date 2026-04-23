using StockTracker.Domain.Common;

namespace StockTracker.Domain.ValueObjects
{
    public sealed class Ticker : ValueObject
    {
        public string Symbol { get; }
        private Ticker(string symbol)
        {
            Symbol = symbol;
        }

        public static Ticker Create(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("Ticker symbol cannot be empty", nameof(symbol));
            
            symbol = symbol.Trim().ToUpperInvariant();
            
            if (symbol.Length < 1 || symbol.Length > 5)
                throw new ArgumentException("Ticker must be 1-5 characters", nameof(symbol));

            if (!symbol.All(char.IsLetter))
                throw new ArgumentException("Ticker must contain only letters", nameof(symbol));

            return new Ticker(symbol);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Symbol;
        }
        public override string ToString() => Symbol;
    }
}
