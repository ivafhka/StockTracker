using StockTracker.Domain.Events;
using StockTracker.Domain.ValueObjects;

namespace StockTracker.Application.Services
{
    internal interface IAlertService
    {
        Task<IReadOnlyList<AlertTriggeredEvent>> EvaluateAlertsForPriceAsync(
            Ticker ticker,
            Money currentPrice,
            CancellationToken cancellationToken = default);
    }
}
