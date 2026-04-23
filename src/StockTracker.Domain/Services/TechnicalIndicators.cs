namespace StockTracker.Domain.Services
{
    public static class TechnicalIndicators
    {
        public static decimal Sma(IReadOnlyList<decimal> prices, int period)
        {
            if (prices is null || prices.Count == 0)
                throw new ArgumentException("Prices cannot be empty", nameof(prices));

            if (period <= 0)
                throw new ArgumentException("Period must be positive", nameof(period));

            if (prices.Count < period)
                throw new ArgumentException($"Not enough data: need {period} points, got {prices.Count}", nameof(prices));

            var lastN = prices.TakeLast(period);
            return lastN.Sum() / period;
        }
        
        public static decimal Ema(IReadOnlyList<decimal> prices, int period)
        {
            if (prices is null || prices.Count == 0)
                throw new ArgumentException("Prices cannot be empty", nameof(prices));

            if (period <= 0)
                throw new ArgumentException("Period must be positive", nameof(period));

            if (prices.Count < period)
                throw new ArgumentException($"Not enough data: need {period} points, got {prices.Count}", nameof(prices));

            var multiplier = 2m / (period + 1);
            var ema = prices.Take(period).Sum() / period;

            for(int i = period; i < prices.Count; i++)
            {
                ema = (prices[i] - ema) * multiplier + ema;
            }

            return ema;
        }
    }
}
