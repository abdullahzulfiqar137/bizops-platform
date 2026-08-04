using BusinessOperations.API.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessOperations.API.Configurations.Identity;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Token)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(rt => rt.Token)
            .IsUnique();

        builder.Property(rt => rt.CreatedByIp)
            .HasMaxLength(50);

        builder.Property(rt => rt.UserAgent)
            .HasMaxLength(300);

        builder.Property(rt => rt.RevokedByIp)
            .HasMaxLength(50);

        builder.Property(rt => rt.ReplacedByToken)
            .HasMaxLength(256);
    }
}
