using MediatR;
using StockTracker.Application.Common;
using StockTracker.Domain.Interfaces;
using StockTracker.Domain.ValueObjects;
using System.Xml.Linq;

namespace StockTracker.Application.Queries.GetPriceHistory
{
    internal class GetPriceHistoryHanlder : IRequestHandler<GetPriceHistoryQuery, Result<IReadOnlyList<PriceTickDto>>>
    {
        private readonly IPriceTickRepository _priceTickRepository;

        public GetPriceHistoryHanlder(IPriceTickRepository priceTickRepository)
        {
            _priceTickRepository = priceTickRepository;
        }
        public async Task<Result<IReadOnlyList<PriceTickDto>>> Handle(
            GetPriceHistoryQuery request,
            CancellationToken cancellationToken)
        {
            Ticker ticker;
            try
            {
                ticker = Ticker.Create(request.Ticker);
            }
            catch( ArgumentException ex)
            {
                return Result.Failure<IReadOnlyList<PriceTickDto>>(ex.Message);
            }

            var ticks = await _priceTickRepository.GetHistoryAsync(
                ticker,
                request.From,
                request.To,
                cancellationToken);

            var dtos = ticks
                .Select(t => new PriceTickDto(
                    Ticker: t.Ticker.Symbol,
                    Price: t.Price.Amount,
                    Currency: t.Price.Currency,
                    Volume: t.Volume,
                    RecordedAt: t.RecordedAt))
                .ToList();

            return Result.Success<IReadOnlyList<PriceTickDto>>(dtos);
        }
    }
}
