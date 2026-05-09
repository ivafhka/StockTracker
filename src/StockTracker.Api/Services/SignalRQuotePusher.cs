using Microsoft.AspNetCore.SignalR;
using StockTracker.Api.Hubs;
using StockTracker.Application.Interfaces;

namespace StockTracker.Api.Services;

public class SignalRQuotePusher : IQuotePusher
{
    private readonly IHubContext<QuoteHub, IQuoteClient> _hubContext;

    public SignalRQuotePusher(IHubContext<QuoteHub, IQuoteClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task PushQuoteAsync(
        string ticker,
        decimal price,
        decimal? sma20,
        decimal? ema12,
        DateTime timestamp,
        CancellationToken cancellationToken = default)
    {
        var update = new QuoteUpdate(ticker, price, sma20, ema12, timestamp);
        await _hubContext.Clients.Group(ticker).ReceiveQuote(update);
    }
}