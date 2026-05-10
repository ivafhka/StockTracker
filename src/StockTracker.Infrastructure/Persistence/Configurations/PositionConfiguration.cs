using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockTracker.Domain.Entities;
using StockTracker.Domain.ValueObjects;

namespace StockTracker.Infrastructure.Persistence.Configurations
{
    public class PositionConfiguration : IEntityTypeConfiguration<Position>
    {
        public void Configure(EntityTypeBuilder<Position> builder)
        {
            builder.ToTable("positions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.PortfolioId).HasColumnName("portfolio_id").IsRequired();
            builder.Property(x => x.quantity).HasColumnName("quantity").HasColumnType("numeric(18,8)");
            builder.Property(x => x.OpenedAt).HasColumnName("opened_at");

            builder.Property(x => x.Ticker)
                .HasColumnName("ticker")
                .HasMaxLength(5)
                .HasConversion(
                    ticker => ticker.Symbol,
                    symbol => Ticker.Create(symbol))
                .IsRequired();

            builder.OwnsOne(x => x.AvgBuyPrice, money =>
            {
                money.Property(x => x.Amount)
                .HasColumnName("avg_buy_price")
                .HasColumnType("numeric(18,8)")
                .IsRequired();

                money.Property(x => x.Currency)
                .HasColumnName("avg_buy_Currency")
                .HasMaxLength(3)
                .IsRequired();
            });

            builder.HasIndex(x => x.Ticker).HasDatabaseName("ix_positions_ticker");
        }
    }
}
