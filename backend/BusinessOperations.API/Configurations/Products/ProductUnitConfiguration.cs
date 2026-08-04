using BusinessOperations.API.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessOperations.API.Configurations.Products;

public sealed class ProductUnitConfiguration : IEntityTypeConfiguration<ProductUnit>
{
    public void Configure(EntityTypeBuilder<ProductUnit> builder)
    {
        builder.ToTable("ProductUnits");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.Abbreviation)
            .HasMaxLength(15)
            .IsRequired();

        builder.HasMany(u => u.Products)
            .WithOne(p => p.Unit)
            .HasForeignKey(p => p.UnitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
