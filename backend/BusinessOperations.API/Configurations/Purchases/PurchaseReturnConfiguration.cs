using BusinessOperations.API.Entities.Purchases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessOperations.API.Configurations.Purchases;

public sealed class PurchaseReturnConfiguration : IEntityTypeConfiguration<PurchaseReturn>
{
    public void Configure(EntityTypeBuilder<PurchaseReturn> builder)
    {
        builder.ToTable("PurchaseReturns");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ReturnNumber).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => new { x.TenantId, x.ReturnNumber }).IsUnique();
        builder.Property(x => x.SubTotal).HasPrecision(18, 2);
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2);
        builder.Property(x => x.DiscountAmount).HasPrecision(18, 2);
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2);
        builder.Property(x => x.SupplierCreditApplied).HasPrecision(18, 2);
        builder.Property(x => x.RefundAmount).HasPrecision(18, 2);
        builder.Property(x => x.Reason).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.HasOne(x => x.Purchase).WithMany(x => x.Returns).HasForeignKey(x => x.PurchaseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Items).WithOne(x => x.PurchaseReturn).HasForeignKey(x => x.PurchaseReturnId).OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class PurchaseReturnItemConfiguration : IEntityTypeConfiguration<PurchaseReturnItem>
{
    public void Configure(EntityTypeBuilder<PurchaseReturnItem> builder)
    {
        builder.ToTable("PurchaseReturnItems");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Quantity).HasPrecision(18, 2);
        builder.Property(x => x.UnitCost).HasPrecision(18, 2);
        builder.Property(x => x.TotalPrice).HasPrecision(18, 2);
        builder.HasOne(x => x.PurchaseItem).WithMany().HasForeignKey(x => x.PurchaseItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
    }
}
