
namespace StockTracker.IntegrationTests.Persistence
{
    [CollectionDefinition(nameof(PostgresCollection))]
    public class PostgresCollection : ICollectionFixture<PostgresFixture>
    {
    }
}
