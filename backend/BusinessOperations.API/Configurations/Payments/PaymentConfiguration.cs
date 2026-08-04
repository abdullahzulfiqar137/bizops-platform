using BusinessOperations.API.Entities.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessOperations.API.Configurations.Payments;

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.PaymentNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(p => new { p.TenantId, p.PaymentNumber })
            .IsUnique();

        builder.Property(p => p.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.ReferenceNumber)
            .HasMaxLength(100);

        builder.Property(p => p.Notes)
            .HasMaxLength(500);

        builder.HasOne(p => p.Customer)
            .WithMany()
            .HasForeignKey(p => p.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Supplier)
            .WithMany()
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Sale)
            .WithMany()
            .HasForeignKey(p => p.SaleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Purchase)
            .WithMany()
            .HasForeignKey(p => p.PurchaseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
