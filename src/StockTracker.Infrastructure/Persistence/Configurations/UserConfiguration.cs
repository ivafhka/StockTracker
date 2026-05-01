using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockTracker.Domain.Entities;

namespace StockTracker.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id");

            builder.Property(x => x.Email)
                .HasColumnName("email")
                .HasMaxLength(256)
                .IsRequired();

            builder.HasIndex(x => x.Email)
                .IsUnique()
                .HasDatabaseName("ix_users_email");

            builder.Property(x => x.PasswordHash)
                .HasColumnName("password_hash")
                .HasMaxLength(512)
                .IsRequired();

            builder.Property(x => x.DisplayName)
                .HasColumnName("display_name")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();
        }
    }
}

