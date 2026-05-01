using MediatR;
using StockTracker.Application.Common;

namespace StockTracker.Application.Queries.GetPortfolio
{
    public record GetPortfolioQuery
        (Guid PortfolioId) : IRequest<Result<PortfolioDto>>;
}
