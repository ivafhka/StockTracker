using MediatR;
using StockTracker.Application.Common;

namespace StockTracker.Application.Queries.GetPriceHistory
{
    public record GetPriceHistoryQuery(
        string Ticker,
        DateTime From,
        DateTime To) : IRequest<Result<IReadOnlyList<PriceTickDto>>>;
}
