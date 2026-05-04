
namespace StockTracker.Infrastructure.Kafka.Configuration
{
    public class KafkaOptions
    {
        public const string SectionName = "Kafka";

        public string BootstrapServers { get; set; } = string.Empty;
        public string GroupIdPrefix {  get; set; } = "stock-tracker";
        public int ProducerLingerMs { get; set; } = 10;
        public int ConsumerSessionTimeoutMs { get; set; } = 30000;
    }
}
