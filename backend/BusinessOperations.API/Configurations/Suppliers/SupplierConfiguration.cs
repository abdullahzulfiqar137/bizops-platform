using BusinessOperations.API.Entities.Suppliers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessOperations.API.Configurations.Suppliers;

public sealed class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.SupplierCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(s => new { s.TenantId, s.SupplierCode })
            .IsUnique();

        builder.Property(s => s.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(s => s.CompanyName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(s => s.Email)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.PhoneNumber)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(s => s.Address)
            .HasMaxLength(250);

        builder.Property(s => s.City)
            .HasMaxLength(100);

        builder.Property(s => s.State)
            .HasMaxLength(100);

        builder.Property(s => s.PostalCode)
            .HasMaxLength(20);

        builder.Property(s => s.Country)
            .HasMaxLength(100);

        builder.Property(s => s.CurrentBalance)
            .HasPrecision(18, 2)
            .IsRequired();
    }
}
