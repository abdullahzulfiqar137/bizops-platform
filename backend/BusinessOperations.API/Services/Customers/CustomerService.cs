using BusinessOperations.API.DTOs.Customers;
using BusinessOperations.API.Data;
using BusinessOperations.API.Entities.Customers;
using BusinessOperations.API.Enums;
using BusinessOperations.API.Interfaces.Repositories;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessOperations.API.Services.Customers;

public sealed class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(
        ICustomerRepository customerRepository,
        ApplicationDbContext context,
        ICurrentUserService currentUserService,
        ILogger<CustomerService> logger)
    {
        _customerRepository = customerRepository;
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<PagedResponse<CustomerResponse>> GetCustomersAsync(
        string? searchTerm,
        string? city,
        bool? isActive,
        string? sortBy,
        bool isDescending,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);

        var (items, totalCount) = await _customerRepository.GetPagedCustomersAsync(
            searchTerm, city, isActive, sortBy, isDescending, pageNumber, pageSize, cancellationToken);

        var responses = items.Select(MapToResponse);

        return new PagedResponse<CustomerResponse>(responses, totalCount, pageNumber, pageSize, "Customers retrieved successfully.");
    }

    public async Task<ApiResponse<CustomerResponse>> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
        if (customer == null)
        {
            return ApiResponse<CustomerResponse>.FailureResponse("Customer not found.");
        }

        return ApiResponse<CustomerResponse>.SuccessResponse(MapToResponse(customer), "Customer details retrieved successfully.");
    }

    public async Task<ApiResponse<CustomerResponse>> CreateCustomerAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var code = await _customerRepository.GenerateNextCustomerCodeAsync(cancellationToken);

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            CustomerCode = code,
            Name = request.Name,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Address = request.Address,
            City = request.City,
            State = request.State,
            PostalCode = request.PostalCode,
            Country = request.Country,
            CurrentBalance = request.OpeningBalance,
            IsActive = true,
            CreatedBy = _currentUserService.Username,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _customerRepository.AddAsync(customer, cancellationToken);
        _logger.LogInformation("Customer created successfully. CustomerId: {Id}, Code: {Code}", customer.Id, customer.CustomerCode);

        return ApiResponse<CustomerResponse>.SuccessResponse(MapToResponse(customer), "Customer created successfully.");
    }

    public async Task<ApiResponse<CustomerResponse>> UpdateCustomerAsync(Guid id, UpdateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
        if (customer == null)
        {
            return ApiResponse<CustomerResponse>.FailureResponse("Customer not found.");
        }

        customer.Name = request.Name;
        customer.Email = request.Email;
        customer.PhoneNumber = request.PhoneNumber;
        customer.Address = request.Address;
        customer.City = request.City;
        customer.State = request.State;
        customer.PostalCode = request.PostalCode;
        customer.Country = request.Country;
        customer.IsActive = request.IsActive;
        customer.UpdatedBy = _currentUserService.Username;
        customer.UpdatedAtUtc = DateTime.UtcNow;

        await _customerRepository.UpdateAsync(customer, cancellationToken);
        _logger.LogInformation("Customer updated. CustomerId: {Id}", customer.Id);

        return ApiResponse<CustomerResponse>.SuccessResponse(MapToResponse(customer), "Customer updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteCustomerAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
        if (customer == null)
        {
            return ApiResponse<bool>.FailureResponse("Customer not found.");
        }

        customer.DeletedBy = _currentUserService.Username;
        await _customerRepository.DeleteAsync(customer, cancellationToken);
        _logger.LogInformation("Customer soft-deleted. CustomerId: {Id}", customer.Id);

        return ApiResponse<bool>.SuccessResponse(true, "Customer deleted successfully.");
    }

    public async Task<ApiResponse<decimal>> GetCustomerBalanceAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
        if (customer == null)
        {
            return ApiResponse<decimal>.FailureResponse("Customer not found.");
        }

        return ApiResponse<decimal>.SuccessResponse(customer.CurrentBalance, "Customer balance retrieved successfully.");
    }

    public async Task<ApiResponse<IEnumerable<CustomerTransactionHistoryResponse>>> GetCustomerTransactionsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
        if (customer == null)
        {
            return ApiResponse<IEnumerable<CustomerTransactionHistoryResponse>>.FailureResponse("Customer not found.");
        }

        var sales = await _context.Sales.AsNoTracking()
            .Where(sale => sale.CustomerId == id && sale.Status != SaleStatus.Cancelled)
            .Select(sale => new HistoryEntry(
                sale.Id, "Sale", sale.InvoiceNumber,
                sale.TotalAmount - (sale.PaidAmount - _context.Payments
                    .Where(payment => payment.SaleId == sale.Id)
                    .Sum(payment => payment.Amount)),
                sale.SaleDateUtc, 1))
            .ToListAsync(cancellationToken);

        var payments = await _context.Payments.AsNoTracking()
            .Where(payment => payment.CustomerId == id && payment.PaymentType == PaymentType.CustomerPayment)
            .Select(payment => new HistoryEntry(
                payment.Id, "Payment", payment.PaymentNumber, payment.Amount,
                payment.PaymentDateUtc, -1))
            .ToListAsync(cancellationToken);

        var returns = await _context.SaleReturns.AsNoTracking()
            .Where(item => item.Sale.CustomerId == id)
            .Select(item => new HistoryEntry(
                item.Id, "Sale Return", item.ReturnNumber, item.CustomerCreditApplied,
                item.ReturnDateUtc, -1))
            .ToListAsync(cancellationToken);

        var entries = sales.Concat(payments).Concat(returns)
            .OrderBy(item => item.DateUtc)
            .ThenBy(item => item.Id)
            .ToList();
        var openingBalance = customer.CurrentBalance - entries.Sum(item => item.Amount * item.Direction);
        var runningBalance = openingBalance;
        var history = entries.Select(item =>
        {
            runningBalance += item.Amount * item.Direction;
            return new CustomerTransactionHistoryResponse
            {
                TransactionId = item.Id,
                TransactionType = item.Type,
                ReferenceNumber = item.Reference,
                Amount = item.Amount,
                BalanceAfter = runningBalance,
                TransactionDateUtc = item.DateUtc
            };
        }).OrderByDescending(item => item.TransactionDateUtc).ToList();

        return ApiResponse<IEnumerable<CustomerTransactionHistoryResponse>>.SuccessResponse(history, "Customer transaction history retrieved.");
    }

    private static CustomerResponse MapToResponse(Customer customer)
    {
        return new CustomerResponse
        {
            Id = customer.Id,
            CustomerCode = customer.CustomerCode,
            Name = customer.Name,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            Address = customer.Address,
            City = customer.City,
            State = customer.State,
            PostalCode = customer.PostalCode,
            Country = customer.Country,
            CurrentBalance = customer.CurrentBalance,
            IsActive = customer.IsActive,
            CreatedAtUtc = customer.CreatedAtUtc
        };
    }

    private sealed record HistoryEntry(
        Guid Id, string Type, string Reference, decimal Amount, DateTime DateUtc, int Direction);
}
