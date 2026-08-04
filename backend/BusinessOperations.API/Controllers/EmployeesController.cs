using BusinessOperations.API.Constants;
using BusinessOperations.API.Data;
using BusinessOperations.API.Entities.Employees;
using BusinessOperations.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Controllers;

public sealed record EmployeeDto(
    Guid Id, string EmployeeCode, string FirstName, string LastName, string Email,
    string Phone, string Department, string Designation, DateTime HireDateUtc,
    decimal MonthlySalary, string Status, string AvatarColor);

public sealed record CreateEmployeeRequest(
    string FirstName, string LastName, string Email, string Phone, string Department,
    string Designation, decimal MonthlySalary, string Status);

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class EmployeesController : ControllerBase
{
    private static readonly string[] AvatarColors =
        ["#2563eb", "#059669", "#7c3aed", "#0891b2", "#be123c", "#b45309"];

    private readonly ApplicationDbContext _context;

    public EmployeesController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<EmployeeDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmployees(
        [FromQuery] string? department,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var query = _context.Employees.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(department) && !department.Equals("ALL", StringComparison.OrdinalIgnoreCase))
            query = query.Where(employee => employee.Department == department);
        if (!string.IsNullOrWhiteSpace(status) && !status.Equals("ALL", StringComparison.OrdinalIgnoreCase))
            query = query.Where(employee => employee.Status == status);

        var employees = await query
            .OrderBy(employee => employee.FirstName)
            .ThenBy(employee => employee.LastName)
            .Select(employee => ToDto(employee))
            .ToListAsync(cancellationToken);

        return Ok(ApiResponse<IEnumerable<EmployeeDto>>.SuccessResponse(employees, "Employee roster retrieved successfully."));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetEmployeeById(Guid id, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees.AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        return employee is null
            ? NotFound(ApiResponse<EmployeeDto>.FailureResponse("Employee record not found."))
            : Ok(ApiResponse<EmployeeDto>.SuccessResponse(ToDto(employee)));
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    public async Task<IActionResult> CreateEmployee(
        [FromBody] CreateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await _context.Employees.AnyAsync(employee => employee.Email == email, cancellationToken))
            return Conflict(ApiResponse<EmployeeDto>.FailureResponse("An employee with this email already exists."));

        var sequence = await _context.Employees.IgnoreQueryFilters()
            .CountAsync(x => x.TenantId == _context.CurrentTenantId, cancellationToken) + 1001;
        var employee = new Employee
        {
            EmployeeCode = $"EMP-{sequence}",
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = email,
            Phone = request.Phone.Trim(),
            Department = request.Department.Trim(),
            Designation = request.Designation.Trim(),
            MonthlySalary = request.MonthlySalary,
            Status = request.Status.Trim(),
            AvatarColor = AvatarColors[(sequence - 1001) % AvatarColors.Length]
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(
            nameof(GetEmployeeById),
            new { id = employee.Id },
            ApiResponse<EmployeeDto>.SuccessResponse(
                ToDto(employee),
                "Employee registered and added to Team Chat automatically."));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteEmployee(Guid id, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (employee is null)
            return NotFound(ApiResponse<bool>.FailureResponse("Employee record not found."));

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok(ApiResponse<bool>.SuccessResponse(true, "Employee removed from the directory and Team Chat."));
    }

    private static EmployeeDto ToDto(Employee employee) =>
        new(
            employee.Id, employee.EmployeeCode, employee.FirstName, employee.LastName,
            employee.Email, employee.Phone, employee.Department, employee.Designation,
            employee.HireDateUtc, employee.MonthlySalary, employee.Status, employee.AvatarColor);
}
