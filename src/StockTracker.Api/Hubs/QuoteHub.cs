using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace StockTracker.Api.Hubs
{
    [Authorize]
    public class QuoteHub : Hub<IQuoteClient>
    {
        private readonly ILogger<QuoteHub> _logger;

        public QuoteHub(ILogger<QuoteHub> logger)
        {
            _logger = logger;
        } 

        public async Task Subscribe(string ticker)
        {
            ticker = ticker.ToUpperInvariant();
            await Groups.AddToGroupAsync(Context.ConnectionId, ticker);
            _logger.LogInformation("Client {ConnectionId} subscribed to {ticker}",
                Context.ConnectionId,ticker);
        }

        public async Task Unsubscribe(string ticker)
        {
            ticker = ticker.ToUpperInvariant();
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, ticker);
            _logger.LogInformation("Client {ConnectionId} unsubscribe from {ticker}",
                Context.ConnectionId, ticker);
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        public async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
