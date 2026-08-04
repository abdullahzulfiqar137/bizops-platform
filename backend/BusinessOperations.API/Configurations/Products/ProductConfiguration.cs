using BusinessOperations.API.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessOperations.API.Configurations.Products;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.SKU)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(p => new { p.TenantId, p.SKU })
            .IsUnique();

        builder.Property(p => p.Barcode)
            .HasMaxLength(100);

        builder.Property(p => p.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.ImageUrl)
            .HasMaxLength(500);

        builder.HasIndex(p => new { p.TenantId, p.Barcode })
            .IsUnique()
            .HasFilter("[Barcode] IS NOT NULL AND [Barcode] <> ''");

        builder.Property(p => p.CostPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.SalePrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.StockQuantity)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.MinimumStockLevel)
            .HasPrecision(18, 2)
            .IsRequired();
    }
}
