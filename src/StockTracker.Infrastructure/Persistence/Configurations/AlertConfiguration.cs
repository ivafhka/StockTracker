using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockTracker.Domain.Entities;
using StockTracker.Domain.ValueObjects;

namespace StockTracker.Infrastructure.Persistence.Configurations
{
    public class AlertConfiguration : IEntityTypeConfiguration<Alert>
    {
        public void Configure(EntityTypeBuilder<Alert> builder)
        {
            builder.ToTable("alerts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
            builder.Property(x => x.IsActive).HasColumnName("is_active").IsRequired();
            builder.Property(x => x.CreatedAt).HasColumnName("created_at");

            builder.Property(x => x.Direction)
                .HasColumnName("direction")
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.Ticker)
                .HasColumnName("ticker")
                .HasMaxLength(5)
                .HasConversion(
                    ticker => ticker.Symbol,
                    symbol => Ticker.Create(symbol))
                .IsRequired();

            builder.OwnsOne(x => x.TargetPrice, money =>
            {
                money.Property(x => x.Amount)
                    .HasColumnName("target_price")
                    .HasColumnType("numeric(18,8)")
                    .IsRequired();

                money.Property(x => x.Currency)
                    .HasColumnName("currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            builder.HasIndex(x => new { x.Ticker, x.IsActive })
                .HasDatabaseName("ix_alerts_ticker_active");

            builder.HasIndex(x => x.UserId).HasDatabaseName("ix_alerts_user_id");
        }
    }
}
