using StockTracker.Application.Interfaces;
using StockTracker.Domain.Events;
using StockTracker.Domain.Interfaces;
using StockTracker.Domain.Services;
using StockTracker.Domain.ValueObjects;

namespace StockTracker.Application.Services
{
    public class AlertService : IAlertService
    {
        private readonly IAlertRepository _alertRepository;
        private readonly IEventBus _eventBus;
        private readonly IUnitOfWork _unitOfWork;

        public AlertService(
            IAlertRepository alertRepository,
            IEventBus eventBus,
            IUnitOfWork unitOfWork)
        {
            _alertRepository = alertRepository;
            _eventBus = eventBus;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<AlertTriggeredEvent>> EvaluateAlertsForPriceAsync(
            Ticker ticker,
            Money currentPrice,
            CancellationToken cancellationToken = default)
        {
            var activeAlerts = await _alertRepository.GetActiveByTickerAsync(ticker, cancellationToken);

            var triggeredEvents = new List<AlertTriggeredEvent>();

            foreach(var alert in activeAlerts)
            {
                var triggeredEvent = AlertEvaluator.Evaluate(alert, currentPrice);

                if (triggeredEvent is null)
                    continue;

                alert.Deactivate();
                await _alertRepository.UpdateAsync(alert, cancellationToken);
                triggeredEvents.Add(triggeredEvent);
            }

            if (triggeredEvents.Count > 0)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                foreach(var domainEvent in triggeredEvents)
                {
                    await _eventBus.PublishAsync(domainEvent, cancellationToken);
                }
            }

            return triggeredEvents;
        }
    }
}
