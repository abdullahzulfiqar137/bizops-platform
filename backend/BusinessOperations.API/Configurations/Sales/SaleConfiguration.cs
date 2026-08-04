using BusinessOperations.API.Entities.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessOperations.API.Configurations.Sales;

public sealed class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.InvoiceNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(s => new { s.TenantId, s.InvoiceNumber })
            .IsUnique();

        builder.Property(s => s.SubTotal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(s => s.TaxRate)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(s => s.TaxAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(s => s.DiscountAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(s => s.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(s => s.PaidAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(s => s.DueAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(s => s.Notes)
            .HasMaxLength(500);

        builder.HasOne(s => s.Customer)
            .WithMany()
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.SaleItems)
            .WithOne(i => i.Sale)
            .HasForeignKey(i => i.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
