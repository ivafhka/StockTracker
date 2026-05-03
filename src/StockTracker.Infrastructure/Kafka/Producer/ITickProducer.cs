
using StockTracker.Infrastructure.Kafka.Models;

namespace StockTracker.Infrastructure.Kafka.Producer
{
    public interface ITickProducer
    {
        Task ProduceAsync(TickMessage tick, CancellationToken cancellationToken = default);
    }
}
