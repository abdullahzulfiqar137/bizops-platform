using BusinessOperations.API.Entities.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessOperations.API.Configurations.Audit;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Action)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.EntityName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.EntityId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.UserName)
            .HasMaxLength(100);

        builder.Property(a => a.IpAddress)
            .HasMaxLength(50);

        builder.Property(a => a.UserAgent)
            .HasMaxLength(500);

        builder.HasIndex(a => a.EntityName);
        builder.HasIndex(a => a.EntityId);
        builder.HasIndex(a => a.TimestampUtc);
    }
}
