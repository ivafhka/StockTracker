using MediatR;
using StockTracker.Application.Common;
using StockTracker.Application.Queries.GetPortfolio;

namespace StockTracker.Application.Queries.GetUserPortfolios
{
    public record GetUserPortfoliosQuery(Guid UserId) : IRequest<Result<IReadOnlyList<PortfolioSummaryDto>>>;
}
