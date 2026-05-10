
namespace StockTracker.Infrastructure.ExternalApis.Finnhub
{
    public class FinnhubOptions
    {
        public const string SectionName = "Finnhub";

        public string ApiKey { get; set; } = string.Empty;
        public string WebSocketUrl {  get; set; } = "wws://ws.finnhub.io";
        public string[] Tickers { get; set; } = Array.Empty<string>();
        public int ReconnectDelayMs { get; set; } = 5000;
    }
}
