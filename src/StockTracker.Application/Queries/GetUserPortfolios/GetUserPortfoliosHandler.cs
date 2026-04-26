using MediatR;
using StockTracker.Application.Common;
using StockTracker.Domain.Interfaces;

namespace StockTracker.Application.Queries.GetUserPortfolios
{
    public class GetUserPortfoliosHandler : IRequestHandler<GetUserPortfoliosQuery, Result<IReadOnlyList<PortfolioSummaryDto>>>
    {
        private readonly IPortfolioRepository _portfolioRepository;

        public GetUserPortfoliosHandler(IPortfolioRepository portfolioRepository)
        {
            _portfolioRepository = portfolioRepository;
        }

        public async Task<Result<IReadOnlyList<PortfolioSummaryDto>>> Handle(
            GetUserPortfoliosQuery request,
            CancellationToken cancellationToken)
        {
            var portfolios = await _portfolioRepository.GetByUserIdAsync(request.UserId, cancellationToken);

            var dtos = portfolios
                .Select(p => new PortfolioSummaryDto(
                    Id: p.Id,
                    Name: p.Name,
                    Description: p.Description,
                    PositionsCount: p.Positions.Count,
                    CreatedAt: p.CreatedAt))
                .ToList();

            return Result.Success<IReadOnlyList<PortfolioSummaryDto>>(dtos);
        }
    }
}
