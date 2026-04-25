using MediatR;
using StockTracker.Application.Common;

namespace StockTracker.Application.Commands.CreatePortfolio
{
    public record CreatePortfolioCommand(
        Guid UserId,
        string Name, 
        string? Description) : IRequest<Result<Guid>>;
}
