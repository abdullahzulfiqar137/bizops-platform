using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using BusinessOperations.API.Entities.Audit;
using BusinessOperations.API.Entities.Accounting;
using BusinessOperations.API.Entities.Businesses;
using BusinessOperations.API.Entities.Common;
using BusinessOperations.API.Entities.Customers;
using BusinessOperations.API.Entities.Expenses;
using BusinessOperations.API.Entities.Employees;
using BusinessOperations.API.Entities.Chat;
using BusinessOperations.API.Entities.Identity;
using BusinessOperations.API.Entities.Inventory;
using BusinessOperations.API.Entities.Notifications;
using BusinessOperations.API.Entities.Payments;
using BusinessOperations.API.Entities.Products;
using BusinessOperations.API.Entities.Purchases;
using BusinessOperations.API.Entities.Sales;
using BusinessOperations.API.Entities.Suppliers;
using BusinessOperations.API.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using BusinessOperations.API.Entities.Insights;
using BusinessOperations.API.Entities.Tenancy;

namespace BusinessOperations.API.Data;

public sealed class ApplicationDbContext : IdentityDbContext<
    ApplicationUser,
    ApplicationRole,
    Guid,
    IdentityUserClaim<Guid>,
    ApplicationUserRole,
    IdentityUserLogin<Guid>,
    IdentityRoleClaim<Guid>,
    IdentityUserToken<Guid>>
{
    private readonly ICurrentUserService? _currentUserService;
    private readonly ITenantContext? _tenantContext;

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<BusinessProfile> BusinessProfiles => Set<BusinessProfile>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<ProductUnit> ProductUnits => Set<ProductUnit>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<InventoryAdjustment> InventoryAdjustments => Set<InventoryAdjustment>();
    public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<SaleReturn> SaleReturns => Set<SaleReturn>();
    public DbSet<SaleReturnItem> SaleReturnItems => Set<SaleReturnItem>();
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<PurchaseItem> PurchaseItems => Set<PurchaseItem>();
    public DbSet<PurchaseReturn> PurchaseReturns => Set<PurchaseReturn>();
    public DbSet<PurchaseReturnItem> PurchaseReturnItems => Set<PurchaseReturnItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<ExpenseCategory> ExpenseCategories => Set<ExpenseCategory>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<TeamChatMessage> TeamChatMessages => Set<TeamChatMessage>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<BusinessOperations.API.Entities.Subscriptions.SubscriptionPlan> SubscriptionPlans => Set<BusinessOperations.API.Entities.Subscriptions.SubscriptionPlan>();
    public DbSet<BusinessOperations.API.Entities.Subscriptions.TenantSubscription> TenantSubscriptions => Set<BusinessOperations.API.Entities.Subscriptions.TenantSubscription>();
    public DbSet<BusinessOperations.API.Entities.Subscriptions.SubscriptionInvoice> SubscriptionInvoices => Set<BusinessOperations.API.Entities.Subscriptions.SubscriptionInvoice>();
    public DbSet<AiChatMessage> AiChatMessages => Set<AiChatMessage>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<JournalLine> JournalLines => Set<JournalLine>();
    public DbSet<UserNotification> UserNotifications => Set<UserNotification>();
    public DbSet<Tenant> Tenants => Set<Tenant>();

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService? currentUserService = null,
        ITenantContext? tenantContext = null) : base(options)
    {
        _currentUserService = currentUserService;
        _tenantContext = tenantContext;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Custom names for Identity tables
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

        modelBuilder.Entity<Tenant>(builder =>
        {
            builder.ToTable("Tenants");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Slug).HasMaxLength(100).IsRequired();
            builder.HasIndex(x => x.Slug).IsUnique();
        });
        modelBuilder.Entity<ApplicationUser>()
            .HasOne(x => x.Tenant).WithMany(x => x.Users).HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<BusinessOperations.API.Entities.Subscriptions.SubscriptionPlan>(builder =>
        {
            builder.Property(x => x.MonthlyPrice).HasPrecision(18, 2);
            builder.Property(x => x.AnnualPrice).HasPrecision(18, 2);
        });
        modelBuilder.Entity<BusinessOperations.API.Entities.Subscriptions.SubscriptionInvoice>()
            .Property(x => x.Amount).HasPrecision(18, 2);

        // Tenant and soft-delete filters are composed so neither protection replaces the other.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            Expression? body = null;
            var parameter = Expression.Parameter(entityType.ClrType, "e");

            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var propertyMethodInfo = typeof(EF).GetMethod(nameof(EF.Property))?.MakeGenericMethod(typeof(bool));
                var isDeletedProperty = Expression.Call(propertyMethodInfo!, parameter, Expression.Constant(nameof(ISoftDeletable.IsDeleted)));
                body = Expression.Equal(isDeletedProperty, Expression.Constant(false));
            }

            var tenantOwned = typeof(ITenantOwned).IsAssignableFrom(entityType.ClrType)
                              && !typeof(IGlobalEntity).IsAssignableFrom(entityType.ClrType);
            if (tenantOwned)
            {
                var tenantProperty = Expression.Call(
                    typeof(EF).GetMethod(nameof(EF.Property))!.MakeGenericMethod(typeof(Guid)),
                    parameter, Expression.Constant(nameof(ITenantOwned.TenantId)));
                var context = Expression.Constant(this);
                var currentTenant = Expression.Property(context, nameof(CurrentTenantId));
                var hasTenant = Expression.Property(context, nameof(HasCurrentTenant));
                var tenantMatch = Expression.Equal(tenantProperty, currentTenant);
                var tenantBody = entityType.ClrType == typeof(ApplicationUser)
                    ? Expression.OrElse(Expression.Not(hasTenant), tenantMatch)
                    : Expression.AndAlso(hasTenant, tenantMatch);
                body = body == null ? tenantBody : Expression.AndAlso(body, tenantBody);
            }

            if (body != null)
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(Expression.Lambda(body, parameter));
        }

        modelBuilder.Entity<ApplicationUserRole>()
            .HasQueryFilter(link => !HasCurrentTenant || link.User.TenantId == CurrentTenantId);
        modelBuilder.Entity<RefreshToken>()
            .HasQueryFilter(token => !HasCurrentTenant || token.TenantId == CurrentTenantId);
    }

    public Guid CurrentTenantId => _tenantContext?.TenantId ?? Guid.Empty;
    public bool HasCurrentTenant => _tenantContext?.HasTenant ?? false;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        var auditEntries = OnBeforeSaveChanges();
        var result = await base.SaveChangesAsync(cancellationToken);

        if (auditEntries.Count > 0)
        {
            try
            {
                await OnAfterSaveChangesAsync(auditEntries, cancellationToken);
            }
            catch
            {
                // Ignore audit log persistence secondary errors
            }
        }

        return result;
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        var auditEntries = OnBeforeSaveChanges();
        var result = base.SaveChanges();

        if (auditEntries.Count > 0)
        {
            OnAfterSaveChangesAsync(auditEntries).GetAwaiter().GetResult();
        }

        return result;
    }

    private void UpdateAuditFields()
    {
        EnforceTenantOwnership();
        var entries = ChangeTracker.Entries<IAuditableEntity>();
        var utcNow = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = utcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAtUtc = utcNow;
            }
        }
    }

    private void EnforceTenantOwnership()
    {
        var tenantId = _tenantContext?.TenantId;
        foreach (var entry in ChangeTracker.Entries<ITenantOwned>()
                     .Where(x => x.Entity is not IGlobalEntity &&
                                 x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted))
        {
            if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
                throw new InvalidOperationException("A tenant context is required for tenant-owned data.");

            if (entry.State == EntityState.Added && entry.Entity.TenantId == Guid.Empty)
                entry.Entity.TenantId = tenantId.Value;

            if (entry.Entity.TenantId != tenantId.Value)
                throw new InvalidOperationException("Cross-tenant data access was blocked.");
        }
    }

    private List<AuditEntry> OnBeforeSaveChanges()
    {
        var auditEntries = new List<AuditEntry>();

        try
        {
            ChangeTracker.DetectChanges();

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is AuditLog || entry.Entity is AiChatMessage || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var auditEntry = new AuditEntry(entry)
                {
                    EntityName = entry.Entity.GetType().Name,
                    UserId = _currentUserService?.UserId,
                    UserName = _currentUserService?.Username ?? "System"
                };

                auditEntries.Add(auditEntry);

                foreach (var property in entry.Properties)
                {
                    if (property.IsTemporary)
                    {
                        auditEntry.TemporaryProperties.Add(property);
                        continue;
                    }

                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.AuditType = "Create";
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            auditEntry.AuditType = "Delete";
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.ChangedColumns.Add(propertyName);
                                auditEntry.AuditType = "Update";
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }
            }

            foreach (var auditEntry in auditEntries.Where(_ => !_.HasTemporaryProperties))
            {
                AuditLogs.Add(auditEntry.ToAuditLog());
            }
        }
        catch
        {
            // Ignore audit trail creation errors so primary operations never fail
        }

        return auditEntries.Where(_ => _.HasTemporaryProperties).ToList();
    }

    private async Task OnAfterSaveChangesAsync(List<AuditEntry> auditEntries, CancellationToken cancellationToken = default)
    {
        try
        {
            foreach (var auditEntry in auditEntries)
            {
                foreach (var prop in auditEntry.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    else
                    {
                        auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }

                AuditLogs.Add(auditEntry.ToAuditLog());
            }

            await base.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // Ignore secondary audit save errors
        }
    }

    private sealed class AuditEntry
    {
        public EntityEntry Entry { get; }
        public Guid? UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public string AuditType { get; set; } = "Create";
        public Dictionary<string, object?> KeyValues { get; } = new();
        public Dictionary<string, object?> OldValues { get; } = new();
        public Dictionary<string, object?> NewValues { get; } = new();
        public List<PropertyEntry> TemporaryProperties { get; } = new();
        public List<string> ChangedColumns { get; } = new();

        public bool HasTemporaryProperties => TemporaryProperties.Count > 0;

        public AuditEntry(EntityEntry entry)
        {
            Entry = entry;
        }

        public AuditLog ToAuditLog()
        {
            var rawUserName = string.IsNullOrWhiteSpace(UserName) ? "System" : UserName;
            var rawAction = string.IsNullOrWhiteSpace(AuditType) ? "Create" : AuditType;
            var rawEntityName = string.IsNullOrWhiteSpace(EntityName) ? "Entity" : EntityName;
            var rawEntityId = JsonSerializer.Serialize(KeyValues);

            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = UserId,
                UserName = rawUserName.Length > 100 ? rawUserName.Substring(0, 100) : rawUserName,
                Action = rawAction.Length > 50 ? rawAction.Substring(0, 50) : rawAction,
                EntityName = rawEntityName.Length > 100 ? rawEntityName.Substring(0, 100) : rawEntityName,
                EntityId = rawEntityId.Length > 100 ? rawEntityId.Substring(0, 100) : rawEntityId,
                OldValues = OldValues.Count == 0 ? null : JsonSerializer.Serialize(OldValues),
                NewValues = NewValues.Count == 0 ? null : JsonSerializer.Serialize(NewValues),
                AffectedColumns = ChangedColumns.Count == 0 ? null : JsonSerializer.Serialize(ChangedColumns),
                TimestampUtc = DateTime.UtcNow
            };

            return auditLog;
        }
    }
}
