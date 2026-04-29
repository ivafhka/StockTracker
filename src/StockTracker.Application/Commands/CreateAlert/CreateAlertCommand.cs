using MediatR;
using StockTracker.Application.Common;
using StockTracker.Domain.Entities;
using StockTracker.Domain.Services;

namespace StockTracker.Application.Commands.CreateAlert
{
    public record CreateAlertCommand(
        Guid UserId,
        string Ticker,
        decimal TargetPrice,
        string Currency,
        AlertDirection Direction) : IRequest<Result<Guid>>;
}
