using BusinessOperations.API.Entities.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessOperations.API.Configurations.Inventory;

public sealed class InventoryAdjustmentConfiguration : IEntityTypeConfiguration<InventoryAdjustment>
{
    public void Configure(EntityTypeBuilder<InventoryAdjustment> builder)
    {
        builder.ToTable("InventoryAdjustments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.AdjustmentNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(a => new { a.TenantId, a.AdjustmentNumber })
            .IsUnique();

        builder.Property(a => a.Quantity)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(a => a.Reason)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.Notes)
            .HasMaxLength(500);

        builder.HasOne(a => a.Product)
            .WithMany()
            .HasForeignKey(a => a.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
