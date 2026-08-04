using BusinessOperations.API.Entities.Purchases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessOperations.API.Configurations.Purchases;

public sealed class PurchaseItemConfiguration : IEntityTypeConfiguration<PurchaseItem>
{
    public void Configure(EntityTypeBuilder<PurchaseItem> builder)
    {
        builder.ToTable("PurchaseItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Quantity)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(i => i.UnitCost)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(i => i.TotalPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
