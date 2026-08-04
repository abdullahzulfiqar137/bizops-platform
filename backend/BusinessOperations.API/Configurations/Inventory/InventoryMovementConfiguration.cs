using BusinessOperations.API.Entities.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessOperations.API.Configurations.Inventory;

public sealed class InventoryMovementConfiguration : IEntityTypeConfiguration<InventoryMovement>
{
    public void Configure(EntityTypeBuilder<InventoryMovement> builder)
    {
        builder.ToTable("InventoryMovements");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.MovementNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(m => new { m.TenantId, m.MovementNumber })
            .IsUnique();

        builder.Property(m => m.Quantity)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(m => m.PreviousStock)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(m => m.NewStock)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(m => m.ReferenceType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(m => m.Notes)
            .HasMaxLength(500);

        builder.HasOne(m => m.Product)
            .WithMany()
            .HasForeignKey(m => m.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
