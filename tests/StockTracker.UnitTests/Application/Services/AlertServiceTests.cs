using FluentAssertions;
using NSubstitute;
using StockTracker.Application.Interfaces;
using StockTracker.Application.Services;
using StockTracker.Domain.Entities;
using StockTracker.Domain.Events;
using StockTracker.Domain.Interfaces;
using StockTracker.Domain.ValueObjects;

namespace StockTracker.UnitTests.Application.Services
{
    public class AlertServiceTests
    {
        private readonly IAlertRepository _alertRepository;
        private readonly IEventBus _eventBus;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AlertService _service;

        public AlertServiceTests()
        {
            _alertRepository = Substitute.For<IAlertRepository>();
            _eventBus = Substitute.For<IEventBus>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _service = new AlertService(_alertRepository, _eventBus, _unitOfWork);
        }

        [Fact]
        public async Task EvaluateAlertsForPriceAsync_ShouldTriggerMatchingAlerts()
        {
            var ticker = Ticker.Create("AAPL");
            var userId = Guid.NewGuid();
            var alert = Alert.Create(userId, ticker, Money.Create(200m), AlertDirection.Above);
            _alertRepository.GetActiveByTickerAsync(ticker, Arg.Any<CancellationToken>())
                .Returns(new List<Alert> { alert });

            var currentPrice = Money.Create(250m);
            var result = await _service.EvaluateAlertsForPriceAsync(ticker, currentPrice);

            result.Should().HaveCount(1);
            await _eventBus.Received(1).PublishAsync(
                Arg.Any<AlertTriggeredEvent>(),
                Arg.Any<CancellationToken>());
            await _unitOfWork.Received().SaveChangesAsync(Arg.Any<CancellationToken>());
        }


        [Fact]
        public async Task EvaluateAlertsForPriceAsync_ShouldTriggerNonMatchingAlerts()
        {
            var ticker = Ticker.Create("AAPL");
            var alert = Alert.Create(Guid.NewGuid(), ticker, Money.Create(200m), AlertDirection.Above);
            _alertRepository.GetActiveByTickerAsync(ticker, Arg.Any<CancellationToken>())
                .Returns(new List<Alert> { alert });

            var currentPrice = Money.Create(150m);
            var result = await _service.EvaluateAlertsForPriceAsync(ticker, currentPrice);

            result.Should().BeEmpty();
            await _eventBus.DidNotReceive().PublishAsync(
                Arg.Any<AlertTriggeredEvent>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task EvaluateAlertsForPriceAsync_ShouldDeactivateTriggeralerts()
        {
            var ticker = Ticker.Create("AAPL");
            var alert = Alert.Create(Guid.NewGuid(), ticker, Money.Create(200m), AlertDirection.Above);
            _alertRepository.GetActiveByTickerAsync(ticker, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IReadOnlyList<Alert>>( new List<Alert> { alert }));

            await _service.EvaluateAlertsForPriceAsync(ticker, Money.Create(250m));

            alert.IsActive.Should().BeFalse();
            await _alertRepository.Received(1).UpdateAsync(Arg.Any<Alert>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task EvaluateAlertsForPirceAsync_ShouldHandleEmptyAlertList()
        {
            var ticker = Ticker.Create("AAPL");
            _alertRepository.GetActiveByTickerAsync(ticker, Arg.Any<CancellationToken>())
                .Returns(new List<Alert>());

            var result = await _service.EvaluateAlertsForPriceAsync(ticker, Money.Create(150m));

            result.Should().BeEmpty();
            await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());

        }
    }
}
