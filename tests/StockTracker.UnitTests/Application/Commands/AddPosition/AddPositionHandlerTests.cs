using FluentAssertions;
using NSubstitute;
using StockTracker.Application.Commands.AddPosition;
using StockTracker.Application.Interfaces;
using StockTracker.Domain.Common;
using StockTracker.Domain.Entities;
using StockTracker.Domain.Events;
using StockTracker.Domain.Interfaces;

namespace StockTracker.UnitTests.Application.Commands.AddPosition
{
    public class AddPositionHandlerTests
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IEventBus _eventBus;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AddPositionHandler _handler;

        public AddPositionHandlerTests()
        {
            _portfolioRepository = Substitute.For<IPortfolioRepository>();
            _eventBus = Substitute.For<IEventBus>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new AddPositionHandler(_portfolioRepository, _eventBus, _unitOfWork);
        }

        [Fact]
        public async Task Handler_ShouldAddPosition_AndPublishEvent()
        {
            var portfolioId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var portfolio = Portfolio.Create(userId, "Test Portfolio");
            _portfolioRepository.GetByIdAsync(portfolioId, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Portfolio?>(portfolio));

            var command = new AddPositionCommand(portfolioId, "AAPL", 10, 150.50m, "USD");
            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBe(Guid.Empty);

            await _eventBus.Received(1).PublishAsync(
                Arg.Any<PositionOpenedEvent>(),
                Arg.Any<CancellationToken>());

            await _portfolioRepository.Received(1).UpdateAsync(Arg.Any<Portfolio>(), Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handler_ShouldFail_WhenPortfolioDoesNotExist()
        {
            var portfolioId = Guid.NewGuid();
            _portfolioRepository.GetByIdAsync(portfolioId, Arg.Any<CancellationToken>());

            var command = new AddPositionCommand(portfolioId, "AAPL", 10, 150m, "USD");
            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("not found");

            await _eventBus.DidNotReceive().PublishAsync(
                Arg.Any<IDomainEvents>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldFail_WhenTickerIsInvalid()
        {
            var portfolioId = Guid.NewGuid();
            var portfolio = Portfolio.Create(Guid.NewGuid(), "Test");
            _portfolioRepository.GetByIdAsync(portfolioId, Arg.Any<CancellationToken>()).Returns(portfolio);

            var command = new AddPositionCommand(portfolioId, "INVALID123", 10, 150m, "USD");
            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ShouldNotPublishEvent_WhenSaveFails()
        {
            var portfolioId = Guid.NewGuid();
            var portfolio = Portfolio.Create(portfolioId, "Test");
            _portfolioRepository.GetByIdAsync(portfolioId, Arg.Any<CancellationToken>()).Returns(portfolio);
            _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns<Task<int>>(_ => throw new Exception("DB error"));

            var command = new AddPositionCommand(portfolioId, "AAPL", 10, 150m, "USD");
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<Exception>();
            await _eventBus.DidNotReceive().PublishAsync(
                Arg.Any<IDomainEvents>(),
                Arg.Any<CancellationToken>());
        }
    }
}
