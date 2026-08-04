using BusinessOperations.API.Entities.Purchases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessOperations.API.Configurations.Purchases;

public sealed class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.ToTable("Purchases");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.PurchaseOrderNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(p => new { p.TenantId, p.PurchaseOrderNumber })
            .IsUnique();

        builder.Property(p => p.SubTotal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.TaxRate)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.TaxAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.DiscountAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.PaidAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.DueAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.Notes)
            .HasMaxLength(500);

        builder.HasOne(p => p.Supplier)
            .WithMany()
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.PurchaseItems)
            .WithOne(i => i.Purchase)
            .HasForeignKey(i => i.PurchaseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
