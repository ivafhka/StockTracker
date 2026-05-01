using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockTracker.Domain.Entities;

namespace StockTracker.Infrastructure.Persistence.Configurations
{
    public class PortfolioConfiguration : IEntityTypeConfiguration<Portfolio>
    {
        public void Configure(EntityTypeBuilder<Portfolio> builder)
        {
            builder.ToTable("portfolios");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).
                HasColumnName("id");
            builder.Property(x => x.UserId).
                HasColumnName("user_id").
                IsRequired();

            builder.Property(x => x.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasColumnName("description")
                .HasMaxLength(500);

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at");

            builder.HasIndex(x => x.UserId)
                .HasDatabaseName("ix_portfolios_user_id");

            builder.HasMany(x => x.Positions)
                .WithOne()
                .HasForeignKey("PortfolioId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.Metadata.FindNavigation(nameof(Portfolio.Positions))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
