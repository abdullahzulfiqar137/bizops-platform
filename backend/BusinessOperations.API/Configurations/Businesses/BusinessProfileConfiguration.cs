using BusinessOperations.API.Entities.Businesses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessOperations.API.Configurations.Businesses;

public sealed class BusinessProfileConfiguration : IEntityTypeConfiguration<BusinessProfile>
{
    public void Configure(EntityTypeBuilder<BusinessProfile> builder)
    {
        builder.ToTable("BusinessProfiles");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.CompanyName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(b => b.LegalName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(b => b.TaxNumber)
            .HasMaxLength(50);

        builder.Property(b => b.Email)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.PhoneNumber)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(b => b.Address)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(b => b.City)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.State)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.PostalCode)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(b => b.Country)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.CurrencyCode)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(b => b.CurrencySymbol)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(b => b.TaxRate)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(b => b.InvoicePrefix)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(b => b.NextInvoiceNumber)
            .IsRequired();

        builder.Property(b => b.LogoUrl)
            .HasMaxLength(500);
    }
}
