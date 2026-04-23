using FluentAssertions;
using StockTracker.Domain.ValueObjects;
using System.Net.NetworkInformation;

namespace StockTracker.UnitTests.Domain.ValueObjects
{
    public class MoneyTests
    {
        [Fact]
        public void Create_ShouldSucceed_WhenValueAreValid()
        {
            var money = Money.Create(100.50m, "USD");

            money.Amount.Should().Be(100.50m);
            money.Currency.Should().Be("USD");
        }

        [Fact]
        public void Create_ShouldUppercaseCurrency()
        {
            var money = Money.Create(100m, "usd");

            money.Currency.Should().Be("USD");
        }

        [Fact]
        public void Create_ShouldDefaultToUsd_WhenCurrencyNotProvided()
        {
            var money = Money.Create(100m);

            money.Currency.Should().Be("USD");
        }

        [Theory]
        [InlineData("US")]
        [InlineData("USDD")]
        [InlineData("")]
        public void Create_ShouldThrow_WhenCurrencyIsInvalid(string currency)
        {
            var act = () => Money.Create(100m, currency);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Zero_ShouldCreateMoneyWithZeroAmount()
        {
            var money = Money.Zero("EUR");

            money.Amount.Should().Be(0m);
            money.Currency.Should().Be("EUR");
        }

        [Fact]
        public void Add_ShouldSumAmounts_WhenCurrenciesMatch()
        {
            var a = Money.Create(100m, "USD");
            var b = Money.Create(50m, "USD");

            var result = a.Add(b);

            result.Amount.Should().Be(150m);
            result.Currency.Should().Be("USD");
        }

        [Fact]
        public void Add_ShouldThrow_WhenCurrenciesDiffer()
        {
            var usd = Money.Create(100m, "USD");
            var eur = Money.Create(50m, "EUR");

            var act = () => usd.Add(eur);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*different currencies*");
        }

        [Fact]
        public void Substract_ShouldReturnDifference()
        {
            var a = Money.Create(100m, "USD");
            var b = Money.Create(30m, "USD");

            var result = a.Subtract(b);

            result.Amount.Should().Be(70m);
        }

        [Fact]
        public void Substract_CanReturnNegative()
        {
            var a = Money.Create(50m, "USD");
            var b = Money.Create(100m, "USD");

            var result = a.Subtract(b);

            result.Amount.Should().Be(-50m);
        }

        [Fact]
        public void Multiply_ShouldMultiplyAmount()
        {
            var money = Money.Create(10m, "USD");

            var result = money.Multiply(5);

            result.Amount.Should().Be(50m);
            result.Currency.Should().Be("USD");
        }

        [Fact]
        public void Operations_ShouldNotModifyOriginal()
        {
            var original = Money.Create(100m, "USD");
            var other = Money.Create(50m, "USD");

            original.Add(other);
            original.Subtract(other);
            original.Multiply(2);

            original.Amount.Should().Be(100m);
        }

        [Fact]
        public void TwoMoneys_ShouldbeEqual_WhenAmountAndCurrencyMatch()
        {
            var a = Money.Create(100m, "USD");
            var b = Money.Create(100m, "USD");

            a.Should().Be(b);
        }

        [Fact]
        public void TwoMoneys_ShouldNotBeEqual_WhenCurrenciesDiffer()
        {
            var usd = Money.Create(100m, "USD");
            var eur = Money.Create(100m, "EUR");

            usd.Should().NotBe(eur);
        }
    }
}
