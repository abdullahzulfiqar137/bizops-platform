using BusinessOperations.API.Entities.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessOperations.API.Configurations.Sales;

public sealed class SaleReturnConfiguration : IEntityTypeConfiguration<SaleReturn>
{
    public void Configure(EntityTypeBuilder<SaleReturn> builder)
    {
        builder.ToTable("SaleReturns");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ReturnNumber).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => new { x.TenantId, x.ReturnNumber }).IsUnique();
        builder.Property(x => x.SubTotal).HasPrecision(18, 2);
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2);
        builder.Property(x => x.DiscountAmount).HasPrecision(18, 2);
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2);
        builder.Property(x => x.CustomerCreditApplied).HasPrecision(18, 2);
        builder.Property(x => x.RefundAmount).HasPrecision(18, 2);
        builder.Property(x => x.Reason).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.HasOne(x => x.Sale).WithMany(x => x.Returns).HasForeignKey(x => x.SaleId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Items).WithOne(x => x.SaleReturn).HasForeignKey(x => x.SaleReturnId).OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class SaleReturnItemConfiguration : IEntityTypeConfiguration<SaleReturnItem>
{
    public void Configure(EntityTypeBuilder<SaleReturnItem> builder)
    {
        builder.ToTable("SaleReturnItems");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Quantity).HasPrecision(18, 2);
        builder.Property(x => x.UnitPrice).HasPrecision(18, 2);
        builder.Property(x => x.TotalPrice).HasPrecision(18, 2);
        builder.HasOne(x => x.SaleItem).WithMany().HasForeignKey(x => x.SaleItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
    }
}
