using StockTracker.Domain.Entities;
using StockTracker.Domain.Events;
using StockTracker.Domain.ValueObjects;

namespace StockTracker.Domain.Services
{
    public static class AlertEvaluator
    {
        public static AlertTriggeredEvent? Evaluate(Alert alert, Money currentPrice)
        {
            ArgumentNullException.ThrowIfNull(alert);
            ArgumentNullException.ThrowIfNull(currentPrice);

            if (!alert.ShouldTrigger(currentPrice))
                return null;

            return new AlertTriggeredEvent(
                AlertId: alert.UserId,
                UserId: alert.UserId,
                Ticker: alert.Ticker,
                TargetPrice: alert.TargetPrice,
                CurrentPrice: currentPrice,
                TriggerAt: DateTime.UtcNow);
        }
    }
}
