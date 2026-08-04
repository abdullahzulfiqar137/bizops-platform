using BusinessOperations.API.Entities.Expenses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessOperations.API.Configurations.Expenses;

public sealed class ExpenseCategoryConfiguration : IEntityTypeConfiguration<ExpenseCategory>
{
    public void Configure(EntityTypeBuilder<ExpenseCategory> builder)
    {
        builder.ToTable("ExpenseCategories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(250);

        builder.HasMany(c => c.Expenses)
            .WithOne(e => e.ExpenseCategory)
            .HasForeignKey(e => e.ExpenseCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
