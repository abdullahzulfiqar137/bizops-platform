using BusinessOperations.API.Entities.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessOperations.API.Configurations.Employees;

public sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");
        builder.HasKey(employee => employee.Id);

        builder.Property(employee => employee.EmployeeCode).HasMaxLength(30).IsRequired();
        builder.HasIndex(employee => new { employee.TenantId, employee.EmployeeCode }).IsUnique();

        builder.Property(employee => employee.FirstName).HasMaxLength(80).IsRequired();
        builder.Property(employee => employee.LastName).HasMaxLength(80).IsRequired();
        builder.Property(employee => employee.Email).HasMaxLength(150).IsRequired();
        builder.HasIndex(employee => new { employee.TenantId, employee.Email }).IsUnique();

        builder.Property(employee => employee.Phone).HasMaxLength(30);
        builder.Property(employee => employee.Department).HasMaxLength(100).IsRequired();
        builder.Property(employee => employee.Designation).HasMaxLength(120).IsRequired();
        builder.Property(employee => employee.MonthlySalary).HasPrecision(18, 2);
        builder.Property(employee => employee.Status).HasMaxLength(30).IsRequired();
        builder.Property(employee => employee.AvatarColor).HasMaxLength(20).IsRequired();
    }
}
