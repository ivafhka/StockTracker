using StockTracker.Domain.Common;

namespace StockTracker.Domain.ValueObjects
{
    public sealed class Money : ValueObject
    {
        public decimal Amount { get; }
        public string Currency { get; }

        private Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }
        public static Money Create(decimal amount, string currency = "USD")
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency is required", nameof(currency));

            if (currency.Length != 3)
                throw new ArgumentException("Currency must be 3-letters ISO code", nameof(currency));

            return new Money(amount, currency.ToUpperInvariant());
        }

        public static Money Zero(string currency = "USD") => new Money(0, currency);

        public Money Add(Money other)
        {
            EnsureSameCurrency(other);
            return new Money(Amount + other.Amount, Currency);
        }

        public Money Subtract(Money other)
        {
            EnsureSameCurrency(other);
            return new Money(Amount - other.Amount, Currency);
        }

        public Money Multiply(decimal factor) => new Money(Amount * factor, Currency);

        private void EnsureSameCurrency(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException(
                    $"Cannot operate on different currencies: {Currency} and {other.Currency}");
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }

        public override string ToString()
        {
            return $"{Amount:0.00} {Currency}";
        }
    }
}
