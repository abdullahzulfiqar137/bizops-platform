using BusinessOperations.API.Entities.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessOperations.API.Configurations.Customers;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.CustomerCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(c => new { c.TenantId, c.CustomerCode })
            .IsUnique();

        builder.Property(c => c.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(c => c.Email)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.PhoneNumber)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(c => c.Address)
            .HasMaxLength(250);

        builder.Property(c => c.City)
            .HasMaxLength(100);

        builder.Property(c => c.State)
            .HasMaxLength(100);

        builder.Property(c => c.PostalCode)
            .HasMaxLength(20);

        builder.Property(c => c.Country)
            .HasMaxLength(100);

        builder.Property(c => c.CurrentBalance)
            .HasPrecision(18, 2)
            .IsRequired();
    }
}
