using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockTracker.Domain.Entities;
using StockTracker.Domain.ValueObjects;

namespace StockTracker.Infrastructure.Persistence.Configurations
{
    public class PriceTickConfiguration : IEntityTypeConfiguration<PriceTick>
    {
        public void Configure(EntityTypeBuilder<PriceTick> builder)
        {
            builder.ToTable("price_ticks");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.Volume).HasColumnName("volume").HasColumnType("numeric(18,8)");
            builder.Property(x => x.RecordedAt).HasColumnName("recorded_at").IsRequired();

            builder.Property(x => x.Ticker)
                .HasColumnName("ticker")
                .HasMaxLength(5)
                .HasConversion(
                    ticker => ticker.Symbol,
                    symbol => Ticker.Create(symbol))
                .IsRequired();

            builder.OwnsOne(x => x.Price, money =>
            {
                money.Property(x => x.Amount)
                    .HasColumnName("price")
                    .HasColumnType("numeric(18,8)")
                    .IsRequired();

                money.Property(x => x.Currency)
                    .HasColumnName("currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            builder.HasIndex(x => new { x.Ticker, x.RecordedAt })
                .IsDescending(false, true)
                .HasDatabaseName("ix_price_ticks_ticker_recorded_at_desc");
        }
    }
}
