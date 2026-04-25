using MediatR;
using StockTracker.Application.Common;
using StockTracker.Application.Interfaces;
using StockTracker.Domain.Events;
using StockTracker.Domain.Interfaces;
using StockTracker.Domain.ValueObjects;

namespace StockTracker.Application.Commands.AddPosition
{
    public class AddPositionHandler : IRequestHandler<AddPositionCommand, Result<Guid>>
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IEventBus _eventBus;
        private readonly IUnitOfWork _unitOfWork;

        public AddPositionHandler(
            IPortfolioRepository portfolioRepository,
            IEventBus eventBus,
            IUnitOfWork unitOfWork)
        {
            _portfolioRepository = portfolioRepository;
            _eventBus = eventBus;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(AddPositionCommand request, CancellationToken cancellationToken)
        {
            var portfolio = await _portfolioRepository.GetByIdAsync(request.PortfolioId, cancellationToken);
            if (portfolio is null)
                return Result.Failure<Guid>($"Portfolio {request.PortfolioId} not found");

            try
            {
                var ticker = Ticker.Create(request.Ticker);
                var buyPrice = Money.Create(request.BuyPrice, request.Currency);

                var position = portfolio.AddPosition(ticker, request.Quanity, buyPrice);

                await _portfolioRepository.UpdateAsync(portfolio, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var domainEvent = new PositionOpenedEvent(
                    PositionId: position.Id,
                    PortfolioId: portfolio.Id,
                    Ticker: ticker,
                    Quanity: request.Quanity,
                    BuyPrice: buyPrice);

                await _eventBus.PublishAsync(domainEvent, cancellationToken);

                return Result.Success(position.Id);
            }
            catch(ArgumentException ex)
            {
                return Result.Failure<Guid>(ex.Message);
            }
        }
    }
}
