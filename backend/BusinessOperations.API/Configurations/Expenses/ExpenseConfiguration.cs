using BusinessOperations.API.Entities.Expenses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessOperations.API.Configurations.Expenses;

public sealed class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable("Expenses");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.ExpenseNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(e => new { e.TenantId, e.ExpenseNumber })
            .IsUnique();

        builder.Property(e => e.Title)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(e => e.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(e => e.ReferenceNumber)
            .HasMaxLength(100);

        builder.Property(e => e.Notes)
            .HasMaxLength(500);

        builder.HasOne(e => e.ExpenseCategory)
            .WithMany(c => c.Expenses)
            .HasForeignKey(e => e.ExpenseCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
