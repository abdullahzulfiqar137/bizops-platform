using BusinessOperations.API.Entities.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessOperations.API.Configurations.Notifications;

public sealed class UserNotificationConfiguration : IEntityTypeConfiguration<UserNotification>
{
    public void Configure(EntityTypeBuilder<UserNotification> builder)
    {
        builder.ToTable("UserNotifications");
        builder.Property(x => x.SourceKey).HasMaxLength(180).IsRequired();
        builder.Property(x => x.Type).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(160).IsRequired();
        builder.Property(x => x.Message).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Route).HasMaxLength(180);
        builder.HasIndex(x => new { x.UserId, x.SourceKey }).IsUnique();
        builder.HasIndex(x => new { x.UserId, x.IsActive, x.IsRead });
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
