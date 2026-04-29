namespace StockTracker.Application.Queries.GetUserPortfolios
{
    public record PortfolioSummaryDto(
        Guid Id,
        string Name,
        string? Description,
        int PositionsCount,
        DateTime CreatedAt);
}
