using BusinessOperations.API.Entities.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessOperations.API.Configurations.Chat;

public sealed class TeamChatMessageConfiguration : IEntityTypeConfiguration<TeamChatMessage>
{
    public void Configure(EntityTypeBuilder<TeamChatMessage> builder)
    {
        builder.ToTable("TeamChatMessages");
        builder.HasKey(message => message.Id);
        builder.Property(message => message.SenderName).HasMaxLength(160).IsRequired();
        builder.Property(message => message.SenderRole).HasMaxLength(80).IsRequired();
        builder.Property(message => message.AvatarColor).HasMaxLength(20).IsRequired();
        builder.Property(message => message.Content).HasMaxLength(4000).IsRequired();
        builder.HasIndex(message => new { message.TargetEmployeeId, message.TimestampUtc });
    }
}
