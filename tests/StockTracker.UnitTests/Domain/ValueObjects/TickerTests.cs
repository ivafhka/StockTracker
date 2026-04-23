using FluentAssertions;
using StockTracker.Domain.ValueObjects;

namespace StockTracker.UnitTests.Domain.ValueObjects
{
    public class TickerTests
    {
        [Theory]
        [InlineData("AAPL")]
        [InlineData("MSFT")]
        [InlineData("A")]
        [InlineData("GOOGL")]
        public void Create_ShouldSucceed_WhenSymbolIsValid(string symbol)
        {
            var ticker = Ticker.Create(symbol);
            ticker.Symbol.Should().Be(symbol);
        }

        [Fact]
        public void Create_ShouldUppercaseSymbol_WhenLowercaseProvided()
        {
            var ticker = Ticker.Create("aapl");
            ticker.Symbol.Should().Be("AAPL");
        }

        [Fact]
        public void Create_ShouldtrimWhitespace()
        {
            var ticker = Ticker.Create("  MSFT  ");
            ticker.Symbol.Should().Be("MSFT");
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void Create_ShouldThrow_WhensymbolIsEmpty(string? symbol)
        {
            var act = () => Ticker.Create(symbol!);
            act.Should().Throw<ArgumentException>()
                .WithMessage("*cannot be empty*");
        }

        [Theory]
        [InlineData("TOOLONG")]
        [InlineData("ABCDEF")]
        public void Create_ShouldThrow_WhenSymbolIsTooLong(string symbol)
        {
            var act = () => Ticker.Create(symbol);
            act.Should().Throw<ArgumentException>()
                .WithMessage("*1-5 characters*");
        }

        [Theory]
        [InlineData("AAP1")]
        [InlineData("AA-PL")]
        [InlineData("123")]
        public void Create_ShouldThrow_WhenSymbolContainsNonLetters(string symbol)
        {
            var act = () => Ticker.Create(symbol);
            act.Should().Throw<ArgumentException>()
                .WithMessage("*only letters*");
        }

        [Fact]
        public void TwoTickers_ShouldBeEqual_WhenSymbolsMatch()
        {
            var ticker1 = Ticker.Create("AAPL");
            var ticker2 = Ticker.Create("AAPL");
            ticker1.Should().Be(ticker2);
            (ticker1 == ticker2).Should().BeTrue();
        }

        [Fact]
        public void TwoTicker_ShouldNotBeEqual_WhenSymbolsDiffer()
        {
            var ticker1 = Ticker.Create("AAPL");
            var ticker2 = Ticker.Create("MSFT");
            ticker1.Should().NotBe(ticker2);
            (ticker1 != ticker2).Should().BeTrue();
        }


    }
}
