using MediatR;
using StockTracker.Application.Common;
using StockTracker.Domain.Interfaces;

namespace StockTracker.Application.Queries.GetPortfolio
{
    public class GetPortfolioHandler : IRequestHandler<GetPortfolioQuery, Result<PortfolioDto>>
    {
        private readonly IPortfolioRepository _portfolioRepository;

        public GetPortfolioHandler(IPortfolioRepository portfolioRepository)
        {
            _portfolioRepository = portfolioRepository;
        }

        public async Task<Result<PortfolioDto>> Handle(GetPortfolioQuery request, CancellationToken cancellationToken)
        {
            var portfolio = await _portfolioRepository.GetByIdAsync(request.PortfolioId, cancellationToken);

            if (portfolio is null)
                return Result.Failure<PortfolioDto>($"Portfolio {request.PortfolioId} not found");

            var positions = portfolio.Positions
                .Select(p => new PositionDto(
                    Id: p.Id,
                    Ticker: p.Ticker.Symbol,
                    Quanity: p.Quanity,
                    AvgbuyPrice: p.AvgBuyPrice.Amount,
                    Currency: p.AvgBuyPrice.Currency,
                    OpenedAt: p.OpenedAt))
                .ToList();

            var dto = new PortfolioDto(
                Id: portfolio.Id,
                UserId: portfolio.UserId,
                Name: portfolio.Name,
                Description: portfolio.Description,
                CreatedAt: portfolio.CreatedAt,
                Positions: positions);

            return Result.Success(dto);
        }
    }
}
