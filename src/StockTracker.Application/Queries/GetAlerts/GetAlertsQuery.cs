using MediatR;
using StockTracker.Application.Common;

namespace StockTracker.Application.Queries.GetAlerts
{
    public record GetAlertsQuery(Guid UserId) : IRequest<Result<IReadOnlyList<AlertDto>>>;
}
