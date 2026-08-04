using System.Text;
using System.Threading.RateLimiting;
using BusinessOperations.API.Configurations;
using BusinessOperations.API.Data;
using BusinessOperations.API.Entities.Identity;
using BusinessOperations.API.Interfaces.Repositories;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Repositories;
using BusinessOperations.API.Services.Ai;
using BusinessOperations.API.Services.Accounting;
using BusinessOperations.API.Services.Audit;
using BusinessOperations.API.Services.Authentication;
using BusinessOperations.API.Services.Tenancy;
using BusinessOperations.API.Services.Businesses;
using BusinessOperations.API.Services.Customers;
using BusinessOperations.API.Services.Expenses;
using BusinessOperations.API.Services.Inventory;
using BusinessOperations.API.Services.Notifications;
using BusinessOperations.API.Services.Imports;
using BusinessOperations.API.Services.Payments;
using BusinessOperations.API.Services.Products;
using BusinessOperations.API.Services.Purchases;
using BusinessOperations.API.Services.Reports;
using BusinessOperations.API.Services.Returns;
using BusinessOperations.API.Services.Sales;
using BusinessOperations.API.Services.Suppliers;
using BusinessOperations.API.Services.Users;
using BusinessOperations.API.Services.Backups;
using BusinessOperations.API.Services.Pdf;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace BusinessOperations.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }

    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;

            options.User.RequireUniqueEmail = true;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = new JwtSettings();
        configuration.GetSection(JwtSettings.SectionName).Bind(jwtSettings);
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey) ||
            jwtSettings.SecretKey.Length < 32)
        {
            throw new InvalidOperationException(
                "JwtSettings:SecretKey must be provided securely and contain at least 32 characters.");
        }

        if (string.IsNullOrWhiteSpace(jwtSettings.Issuer) ||
            string.IsNullOrWhiteSpace(jwtSettings.Audience))
        {
            throw new InvalidOperationException(
                "JwtSettings:Issuer and JwtSettings:Audience are required.");
        }

        var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = jwtSettings.RequireHttpsMetadata;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }

    public static IServiceCollection AddSecurityAndCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var allowedOrigins = configuration
            .GetSection("AllowedOrigins")
            .Get<string[]>()
            ?? Array.Empty<string>();

        services.AddCors(options =>
        {
            options.AddPolicy("Frontend", builder =>
            {
                if (allowedOrigins.Length > 0)
                {
                    builder.WithOrigins(allowedOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                }
                else
                {
                    throw new InvalidOperationException(
                        "At least one explicit AllowedOrigins entry is required when credentialed CORS is enabled.");
                }
            });
        });

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddFixedWindowLimiter("FixedPolicy", opt =>
            {
                opt.Window = TimeSpan.FromSeconds(10);
                opt.PermitLimit = 100;
                opt.QueueLimit = 10;
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });
            options.AddFixedWindowLimiter("AuthPolicy", opt =>
            {
                opt.Window = TimeSpan.FromMinutes(1);
                opt.PermitLimit = 8;
                opt.QueueLimit = 0;
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });
        });

        services.AddMemoryCache();

        return services;
    }

    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<EmailSettings>(
            configuration.GetSection(EmailSettings.SectionName));
        services.Configure<BackupSettings>(
            configuration.GetSection(BackupSettings.SectionName));
        services.AddHttpContextAccessor();
        services.AddScoped<ITenantContext, TenantContext>();

        // Repositories
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IInventoryAdjustmentRepository, InventoryAdjustmentRepository>();
        services.AddScoped<IInventoryMovementRepository, InventoryMovementRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<IPurchaseRepository, PurchaseRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IExpenseCategoryRepository, ExpenseCategoryRepository>();
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();

        // Services
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IAccountingService, AccountingService>();
        services.AddScoped<ITokenGeneratorService, TokenGeneratorService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddScoped<IDatabaseBackupService, DatabaseBackupService>();
        services.AddHostedService<ScheduledBackupService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBusinessProfileService, BusinessProfileService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProductMediaService, ProductMediaService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<ICsvImportService, CsvImportService>();
        services.AddScoped<ISaleService, SaleService>();
        services.AddScoped<IPurchaseService, PurchaseService>();
        services.AddScoped<IReturnService, ReturnService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IExpenseCategoryService, ExpenseCategoryService>();
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<IAiInsightService, AiInsightService>();
        services.AddScoped<IPdfDocumentService, PdfDocumentService>();
        services.AddHttpClient();

        services.AddValidatorsFromAssembly(typeof(Program).Assembly);

        return services;
    }

    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Business Operations API",
                Version = "v1",
                Description = "AI-Powered Business Operations System Backend API",
                Contact = new OpenApiContact
                {
                    Name = "Business Operations Team"
                }
            });

            // Add JWT Security Definition to Swagger UI
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter JWT Bearer token format: Bearer {your token}"
            };

            options.AddSecurityDefinition("Bearer", securityScheme);

            var requirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("Bearer"),
                    new List<string>()
                }
            };

            options.AddSecurityRequirement(doc => requirement);

            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }
}
