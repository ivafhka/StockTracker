using StockTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockTracker.Application.Queries.GetPortfolio
{
    public record PortfolioDto(
        Guid Id,
        Guid UserId,
        string Name,
        string? Description,
        DateTime CreatedAt,
        IReadOnlyList<PositionDto> Positions);

    public record PositionDto(
        Guid Id,
        string Ticker,
        decimal quantity,
        decimal AvgbuyPrice,
        string Currency,
        DateTime OpenedAt);
}
