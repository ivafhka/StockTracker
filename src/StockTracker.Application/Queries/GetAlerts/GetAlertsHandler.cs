using MediatR;
using StockTracker.Application.Common;
using StockTracker.Domain.Interfaces;

namespace StockTracker.Application.Queries.GetAlerts;

public class GetAlertsHandler : IRequestHandler<GetAlertsQuery, Result<IReadOnlyList<AlertDto>>>
{
    private readonly IAlertRepository _alertRepository;

    public GetAlertsHandler(IAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    public async Task<Result<IReadOnlyList<AlertDto>>> Handle(
        GetAlertsQuery request,
        CancellationToken cancellationToken)
    {
        var alerts = await _alertRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        var dtos = alerts
            .Select(a => new AlertDto(
                Id: a.Id,
                Ticker: a.Ticker.Symbol,
                TargetPrice: a.TargetPrice.Amount,
                Currency: a.TargetPrice.Currency,
                Direction: a.Direction,
                IsActive: a.IsActive,
                CreatedAt: a.CreatedAt))
            .ToList();

        return Result.Success<IReadOnlyList<AlertDto>>(dtos);
    }
}