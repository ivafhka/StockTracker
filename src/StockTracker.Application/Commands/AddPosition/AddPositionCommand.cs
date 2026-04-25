using MediatR;
using StockTracker.Application.Common;

namespace StockTracker.Application.Commands.AddPosition
{
    public record AddPositionCommand(
        Guid PortfolioId,
        string Ticker,
        decimal Quanity,
        decimal BuyPrice,
        string Currency) : IRequest<Result<Guid>>;
}
