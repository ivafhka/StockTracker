
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StockTracker.Domain.Entities;
using StockTracker.Domain.ValueObjects;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace StockTracker.Infrastructure.ExternalApis.Finnhub
{
    public class FinnhubWebSocketClient : IDisposable
    {
        private readonly FinnhubOptions _options;
        private readonly ILogger<FinnhubWebSocketClient> _logger;
        private ClientWebSocket? _socket;

        public FinnhubWebSocketClient(IOptions<FinnhubOptions> options, ILogger<FinnhubWebSocketClient> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            _socket = new ClientWebSocket();
            var uri = new Uri($"{_options.WebSocketUrl}?token{_options.ApiKey}");

            await _socket.ConnectAsync(uri, cancellationToken);
            _logger.LogInformation("Connected to Finnhub WebSocket");

            foreach(var ticker in _options.Tickers)
            {
                var subscribeMessage = JsonSerializer.Serialize(new { type = "subscribe", symbol = ticker });
                await SendAsync(subscribeMessage, cancellationToken);
                _logger.LogInformation("Subscribed to {Ticker", ticker);
            }
        } 

        public async IAsyncEnumerable<FinnhubTickEvent> ReciveTicksAsync(
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (_socket is null)
                throw new InvalidOperationException("WebSocket is not connected. Call ConnectAsync first");

            var buffer = new byte[8192];

            while(_socket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                var messageBuilder = new StringBuilder();
                WebSocketReceiveResult result;

                do
                {
                    result = await _socket.ReceiveAsync(buffer, cancellationToken);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        _logger.LogWarning("WebSocket closed by server");
                        yield break;
                    }

                    messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                }
                while (!result.EndOfMessage);

                var json = messageBuilder.ToString();
                FinnhubMessage? message = null;

                try
                {
                    message = JsonSerializer.Deserialize<FinnhubMessage>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "Failed to parse Finnhub message: {Json}", json);
                    continue;
                }

                if (message?.Type != "trade" || message.Data is null)
                    continue;

                foreach(var trade in message.Data)
                {
                    yield return new FinnhubTickEvent(
                        Ticker: trade.S,
                        Price: trade.P,
                        Volume: trade.V,
                        TimestampMs: trade.T);
                }
            }
        }

        private async Task SendAsync(string message, CancellationToken cancellationToken)
        {
            if (_socket is null) return;
            var bytes = Encoding.UTF8.GetBytes(message);
            await _socket.SendAsync(bytes, WebSocketMessageType.Text, true, cancellationToken);
        }
        public void Dispose()
        {
            _socket?.Dispose();
        }

        private record FinnhubMessage(string Type, FinnhubTrade[]? Data);
        private record FinnhubTrade(string S, decimal P, decimal V, long T);
    }

    public record FinnhubTickEvent(string Ticker, decimal Price, decimal Volume, long TimestampMs)
    {
        public DateTime RecordedAt => DateTimeOffset.FromUnixTimeMilliseconds(TimestampMs).UtcDateTime;
    }
}
