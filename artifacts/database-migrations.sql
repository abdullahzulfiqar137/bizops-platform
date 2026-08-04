IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727080043_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260727080043_InitialCreate', N'10.0.10');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727080840_AddIdentityAndRefreshTokens'
)
BEGIN
    CREATE TABLE [Roles] (
        [Id] uniqueidentifier NOT NULL,
        [Description] nvarchar(250) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727080840_AddIdentityAndRefreshTokens'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] uniqueidentifier NOT NULL,
        [FirstName] nvarchar(50) NOT NULL,
        [LastName] nvarchar(50) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727080840_AddIdentityAndRefreshTokens'
)
BEGIN
    CREATE TABLE [RoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] uniqueidentifier NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_RoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RoleClaims_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727080840_AddIdentityAndRefreshTokens'
)
BEGIN
    CREATE TABLE [RefreshTokens] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [Token] nvarchar(256) NOT NULL,
        [ExpiresAtUtc] datetime2 NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedByIp] nvarchar(50) NOT NULL,
        [RevokedAtUtc] datetime2 NULL,
        [RevokedByIp] nvarchar(50) NULL,
        [ReplacedByToken] nvarchar(256) NULL,
        CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RefreshTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727080840_AddIdentityAndRefreshTokens'
)
BEGIN
    CREATE TABLE [UserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] uniqueidentifier NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_UserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserClaims_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727080840_AddIdentityAndRefreshTokens'
)
BEGIN
    CREATE TABLE [UserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_UserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_UserLogins_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727080840_AddIdentityAndRefreshTokens'
)
BEGIN
    CREATE TABLE [UserRoles] (
        [UserId] uniqueidentifier NOT NULL,
        [RoleId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_UserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727080840_AddIdentityAndRefreshTokens'
)
BEGIN
    CREATE TABLE [UserTokens] (
        [UserId] uniqueidentifier NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_UserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_UserTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727080840_AddIdentityAndRefreshTokens'
)
BEGIN
    CREATE UNIQUE INDEX [IX_RefreshTokens_Token] ON [RefreshTokens] ([Token]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727080840_AddIdentityAndRefreshTokens'
)
BEGIN
    CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727080840_AddIdentityAndRefreshTokens'
)
BEGIN
    CREATE INDEX [IX_RoleClaims_RoleId] ON [RoleClaims] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727080840_AddIdentityAndRefreshTokens'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [Roles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727080840_AddIdentityAndRefreshTokens'
)
BEGIN
    CREATE INDEX [IX_UserClaims_UserId] ON [UserClaims] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727080840_AddIdentityAndRefreshTokens'
)
BEGIN
    CREATE INDEX [IX_UserLogins_UserId] ON [UserLogins] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727080840_AddIdentityAndRefreshTokens'
)
BEGIN
    CREATE INDEX [IX_UserRoles_RoleId] ON [UserRoles] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727080840_AddIdentityAndRefreshTokens'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [Users] ([NormalizedEmail]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727080840_AddIdentityAndRefreshTokens'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [Users] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727080840_AddIdentityAndRefreshTokens'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260727080840_AddIdentityAndRefreshTokens', N'10.0.10');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727081530_AddBusinessProfile'
)
BEGIN
    CREATE TABLE [BusinessProfiles] (
        [Id] uniqueidentifier NOT NULL,
        [CompanyName] nvarchar(150) NOT NULL,
        [LegalName] nvarchar(150) NOT NULL,
        [TaxNumber] nvarchar(50) NULL,
        [Email] nvarchar(100) NOT NULL,
        [PhoneNumber] nvarchar(30) NOT NULL,
        [Address] nvarchar(250) NOT NULL,
        [City] nvarchar(100) NOT NULL,
        [State] nvarchar(100) NOT NULL,
        [PostalCode] nvarchar(20) NOT NULL,
        [Country] nvarchar(100) NOT NULL,
        [CurrencyCode] nvarchar(10) NOT NULL,
        [CurrencySymbol] nvarchar(10) NOT NULL,
        [TaxRate] decimal(18,2) NOT NULL,
        [InvoicePrefix] nvarchar(20) NOT NULL,
        [NextInvoiceNumber] bigint NOT NULL,
        [LogoUrl] nvarchar(500) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_BusinessProfiles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727081530_AddBusinessProfile'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260727081530_AddBusinessProfile', N'10.0.10');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727081927_AddCustomers'
)
BEGIN
    CREATE TABLE [Customers] (
        [Id] uniqueidentifier NOT NULL,
        [CustomerCode] nvarchar(50) NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [Email] nvarchar(100) NOT NULL,
        [PhoneNumber] nvarchar(30) NOT NULL,
        [Address] nvarchar(250) NULL,
        [City] nvarchar(100) NULL,
        [State] nvarchar(100) NULL,
        [PostalCode] nvarchar(20) NULL,
        [Country] nvarchar(100) NULL,
        [CurrentBalance] decimal(18,2) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Customers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727081927_AddCustomers'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Customers_CustomerCode] ON [Customers] ([CustomerCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727081927_AddCustomers'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260727081927_AddCustomers', N'10.0.10');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727082240_AddSuppliers'
)
BEGIN
    CREATE TABLE [Suppliers] (
        [Id] uniqueidentifier NOT NULL,
        [SupplierCode] nvarchar(50) NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [CompanyName] nvarchar(150) NOT NULL,
        [Email] nvarchar(100) NOT NULL,
        [PhoneNumber] nvarchar(30) NOT NULL,
        [Address] nvarchar(250) NULL,
        [City] nvarchar(100) NULL,
        [State] nvarchar(100) NULL,
        [PostalCode] nvarchar(20) NULL,
        [Country] nvarchar(100) NULL,
        [CurrentBalance] decimal(18,2) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Suppliers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727082240_AddSuppliers'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Suppliers_SupplierCode] ON [Suppliers] ([SupplierCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727082240_AddSuppliers'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260727082240_AddSuppliers', N'10.0.10');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727083204_AddCategoriesAndProducts'
)
BEGIN
    CREATE TABLE [Categories] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Description] nvarchar(250) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Categories] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727083204_AddCategoriesAndProducts'
)
BEGIN
    CREATE TABLE [ProductUnits] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(50) NOT NULL,
        [Abbreviation] nvarchar(15) NOT NULL,
        CONSTRAINT [PK_ProductUnits] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727083204_AddCategoriesAndProducts'
)
BEGIN
    CREATE TABLE [Products] (
        [Id] uniqueidentifier NOT NULL,
        [SKU] nvarchar(50) NOT NULL,
        [Barcode] nvarchar(100) NULL,
        [Name] nvarchar(150) NOT NULL,
        [Description] nvarchar(500) NULL,
        [CategoryId] uniqueidentifier NOT NULL,
        [UnitId] uniqueidentifier NOT NULL,
        [CostPrice] decimal(18,2) NOT NULL,
        [SalePrice] decimal(18,2) NOT NULL,
        [StockQuantity] decimal(18,2) NOT NULL,
        [MinimumStockLevel] decimal(18,2) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Products] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Products_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Products_ProductUnits_UnitId] FOREIGN KEY ([UnitId]) REFERENCES [ProductUnits] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727083204_AddCategoriesAndProducts'
)
BEGIN
    CREATE INDEX [IX_Products_CategoryId] ON [Products] ([CategoryId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727083204_AddCategoriesAndProducts'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Products_SKU] ON [Products] ([SKU]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727083204_AddCategoriesAndProducts'
)
BEGIN
    CREATE INDEX [IX_Products_UnitId] ON [Products] ([UnitId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727083204_AddCategoriesAndProducts'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260727083204_AddCategoriesAndProducts', N'10.0.10');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727083528_AddInventoryManagement'
)
BEGIN
    CREATE TABLE [InventoryAdjustments] (
        [Id] uniqueidentifier NOT NULL,
        [AdjustmentNumber] nvarchar(50) NOT NULL,
        [ProductId] uniqueidentifier NOT NULL,
        [AdjustmentType] int NOT NULL,
        [Quantity] decimal(18,2) NOT NULL,
        [Reason] nvarchar(200) NOT NULL,
        [Notes] nvarchar(500) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_InventoryAdjustments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_InventoryAdjustments_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727083528_AddInventoryManagement'
)
BEGIN
    CREATE TABLE [InventoryMovements] (
        [Id] uniqueidentifier NOT NULL,
        [MovementNumber] nvarchar(50) NOT NULL,
        [ProductId] uniqueidentifier NOT NULL,
        [MovementType] int NOT NULL,
        [Quantity] decimal(18,2) NOT NULL,
        [PreviousStock] decimal(18,2) NOT NULL,
        [NewStock] decimal(18,2) NOT NULL,
        [ReferenceType] nvarchar(50) NOT NULL,
        [ReferenceId] uniqueidentifier NOT NULL,
        [Notes] nvarchar(500) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_InventoryMovements] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_InventoryMovements_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727083528_AddInventoryManagement'
)
BEGIN
    CREATE UNIQUE INDEX [IX_InventoryAdjustments_AdjustmentNumber] ON [InventoryAdjustments] ([AdjustmentNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727083528_AddInventoryManagement'
)
BEGIN
    CREATE INDEX [IX_InventoryAdjustments_ProductId] ON [InventoryAdjustments] ([ProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727083528_AddInventoryManagement'
)
BEGIN
    CREATE UNIQUE INDEX [IX_InventoryMovements_MovementNumber] ON [InventoryMovements] ([MovementNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727083528_AddInventoryManagement'
)
BEGIN
    CREATE INDEX [IX_InventoryMovements_ProductId] ON [InventoryMovements] ([ProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727083528_AddInventoryManagement'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260727083528_AddInventoryManagement', N'10.0.10');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727083946_AddSalesManagement'
)
BEGIN
    CREATE TABLE [Sales] (
        [Id] uniqueidentifier NOT NULL,
        [InvoiceNumber] nvarchar(50) NOT NULL,
        [CustomerId] uniqueidentifier NOT NULL,
        [SaleDateUtc] datetime2 NOT NULL,
        [SubTotal] decimal(18,2) NOT NULL,
        [TaxRate] decimal(18,2) NOT NULL,
        [TaxAmount] decimal(18,2) NOT NULL,
        [DiscountAmount] decimal(18,2) NOT NULL,
        [TotalAmount] decimal(18,2) NOT NULL,
        [PaidAmount] decimal(18,2) NOT NULL,
        [DueAmount] decimal(18,2) NOT NULL,
        [Status] int NOT NULL,
        [PaymentStatus] int NOT NULL,
        [Notes] nvarchar(500) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Sales] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Sales_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727083946_AddSalesManagement'
)
BEGIN
    CREATE TABLE [SaleItems] (
        [Id] uniqueidentifier NOT NULL,
        [SaleId] uniqueidentifier NOT NULL,
        [ProductId] uniqueidentifier NOT NULL,
        [Quantity] decimal(18,2) NOT NULL,
        [UnitPrice] decimal(18,2) NOT NULL,
        [TotalPrice] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_SaleItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SaleItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_SaleItems_Sales_SaleId] FOREIGN KEY ([SaleId]) REFERENCES [Sales] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727083946_AddSalesManagement'
)
BEGIN
    CREATE INDEX [IX_SaleItems_ProductId] ON [SaleItems] ([ProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727083946_AddSalesManagement'
)
BEGIN
    CREATE INDEX [IX_SaleItems_SaleId] ON [SaleItems] ([SaleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727083946_AddSalesManagement'
)
BEGIN
    CREATE INDEX [IX_Sales_CustomerId] ON [Sales] ([CustomerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727083946_AddSalesManagement'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Sales_InvoiceNumber] ON [Sales] ([InvoiceNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727083946_AddSalesManagement'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260727083946_AddSalesManagement', N'10.0.10');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727085903_AddPurchaseManagement'
)
BEGIN
    CREATE TABLE [Purchases] (
        [Id] uniqueidentifier NOT NULL,
        [PurchaseOrderNumber] nvarchar(50) NOT NULL,
        [SupplierId] uniqueidentifier NOT NULL,
        [PurchaseDateUtc] datetime2 NOT NULL,
        [SubTotal] decimal(18,2) NOT NULL,
        [TaxRate] decimal(18,2) NOT NULL,
        [TaxAmount] decimal(18,2) NOT NULL,
        [DiscountAmount] decimal(18,2) NOT NULL,
        [TotalAmount] decimal(18,2) NOT NULL,
        [PaidAmount] decimal(18,2) NOT NULL,
        [DueAmount] decimal(18,2) NOT NULL,
        [Status] int NOT NULL,
        [PaymentStatus] int NOT NULL,
        [Notes] nvarchar(500) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Purchases] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Purchases_Suppliers_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [Suppliers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727085903_AddPurchaseManagement'
)
BEGIN
    CREATE TABLE [PurchaseItems] (
        [Id] uniqueidentifier NOT NULL,
        [PurchaseId] uniqueidentifier NOT NULL,
        [ProductId] uniqueidentifier NOT NULL,
        [Quantity] decimal(18,2) NOT NULL,
        [UnitCost] decimal(18,2) NOT NULL,
        [TotalPrice] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_PurchaseItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PurchaseItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PurchaseItems_Purchases_PurchaseId] FOREIGN KEY ([PurchaseId]) REFERENCES [Purchases] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727085903_AddPurchaseManagement'
)
BEGIN
    CREATE INDEX [IX_PurchaseItems_ProductId] ON [PurchaseItems] ([ProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727085903_AddPurchaseManagement'
)
BEGIN
    CREATE INDEX [IX_PurchaseItems_PurchaseId] ON [PurchaseItems] ([PurchaseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727085903_AddPurchaseManagement'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Purchases_PurchaseOrderNumber] ON [Purchases] ([PurchaseOrderNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727085903_AddPurchaseManagement'
)
BEGIN
    CREATE INDEX [IX_Purchases_SupplierId] ON [Purchases] ([SupplierId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727085903_AddPurchaseManagement'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260727085903_AddPurchaseManagement', N'10.0.10');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727091322_AddPaymentProcessing'
)
BEGIN
    CREATE TABLE [Payments] (
        [Id] uniqueidentifier NOT NULL,
        [PaymentNumber] nvarchar(50) NOT NULL,
        [PaymentType] int NOT NULL,
        [CustomerId] uniqueidentifier NULL,
        [SupplierId] uniqueidentifier NULL,
        [SaleId] uniqueidentifier NULL,
        [PurchaseId] uniqueidentifier NULL,
        [Amount] decimal(18,2) NOT NULL,
        [PaymentDateUtc] datetime2 NOT NULL,
        [PaymentMethod] int NOT NULL,
        [ReferenceNumber] nvarchar(100) NULL,
        [Notes] nvarchar(500) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Payments_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Payments_Purchases_PurchaseId] FOREIGN KEY ([PurchaseId]) REFERENCES [Purchases] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Payments_Sales_SaleId] FOREIGN KEY ([SaleId]) REFERENCES [Sales] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Payments_Suppliers_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [Suppliers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727091322_AddPaymentProcessing'
)
BEGIN
    CREATE INDEX [IX_Payments_CustomerId] ON [Payments] ([CustomerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727091322_AddPaymentProcessing'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Payments_PaymentNumber] ON [Payments] ([PaymentNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727091322_AddPaymentProcessing'
)
BEGIN
    CREATE INDEX [IX_Payments_PurchaseId] ON [Payments] ([PurchaseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727091322_AddPaymentProcessing'
)
BEGIN
    CREATE INDEX [IX_Payments_SaleId] ON [Payments] ([SaleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727091322_AddPaymentProcessing'
)
BEGIN
    CREATE INDEX [IX_Payments_SupplierId] ON [Payments] ([SupplierId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727091322_AddPaymentProcessing'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260727091322_AddPaymentProcessing', N'10.0.10');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727102440_AddExpenseManagement'
)
BEGIN
    CREATE TABLE [ExpenseCategories] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Description] nvarchar(250) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_ExpenseCategories] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727102440_AddExpenseManagement'
)
BEGIN
    CREATE TABLE [Expenses] (
        [Id] uniqueidentifier NOT NULL,
        [ExpenseNumber] nvarchar(50) NOT NULL,
        [ExpenseCategoryId] uniqueidentifier NOT NULL,
        [Title] nvarchar(150) NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [ExpenseDateUtc] datetime2 NOT NULL,
        [PaymentMethod] int NOT NULL,
        [ReferenceNumber] nvarchar(100) NULL,
        [Notes] nvarchar(500) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Expenses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Expenses_ExpenseCategories_ExpenseCategoryId] FOREIGN KEY ([ExpenseCategoryId]) REFERENCES [ExpenseCategories] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727102440_AddExpenseManagement'
)
BEGIN
    CREATE INDEX [IX_Expenses_ExpenseCategoryId] ON [Expenses] ([ExpenseCategoryId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727102440_AddExpenseManagement'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Expenses_ExpenseNumber] ON [Expenses] ([ExpenseNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727102440_AddExpenseManagement'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260727102440_AddExpenseManagement', N'10.0.10');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727103111_AddAuditLoggingFramework'
)
BEGIN
    CREATE TABLE [AuditLogs] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NULL,
        [UserName] nvarchar(100) NULL,
        [Action] nvarchar(50) NOT NULL,
        [EntityName] nvarchar(100) NOT NULL,
        [EntityId] nvarchar(100) NOT NULL,
        [OldValues] nvarchar(max) NULL,
        [NewValues] nvarchar(max) NULL,
        [AffectedColumns] nvarchar(max) NULL,
        [IpAddress] nvarchar(50) NULL,
        [UserAgent] nvarchar(500) NULL,
        [TimestampUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727103111_AddAuditLoggingFramework'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_EntityId] ON [AuditLogs] ([EntityId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727103111_AddAuditLoggingFramework'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_EntityName] ON [AuditLogs] ([EntityName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727103111_AddAuditLoggingFramework'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_TimestampUtc] ON [AuditLogs] ([TimestampUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727103111_AddAuditLoggingFramework'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260727103111_AddAuditLoggingFramework', N'10.0.10');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727182715_AddAiChatMessagesTable'
)
BEGIN
    CREATE TABLE [AiChatMessages] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] nvarchar(max) NOT NULL,
        [UserPrompt] nvarchar(max) NOT NULL,
        [AiResponse] nvarchar(max) NOT NULL,
        [TimestampUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_AiChatMessages] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727182715_AddAiChatMessagesTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260727182715_AddAiChatMessagesTable', N'10.0.10');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730063025_AddSalesAndPurchaseReturns'
)
BEGIN
    CREATE TABLE [PurchaseReturns] (
        [Id] uniqueidentifier NOT NULL,
        [ReturnNumber] nvarchar(50) NOT NULL,
        [PurchaseId] uniqueidentifier NOT NULL,
        [ReturnDateUtc] datetime2 NOT NULL,
        [SubTotal] decimal(18,2) NOT NULL,
        [TaxAmount] decimal(18,2) NOT NULL,
        [DiscountAmount] decimal(18,2) NOT NULL,
        [TotalAmount] decimal(18,2) NOT NULL,
        [SupplierCreditApplied] decimal(18,2) NOT NULL,
        [RefundAmount] decimal(18,2) NOT NULL,
        [Reason] nvarchar(250) NOT NULL,
        [Notes] nvarchar(500) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_PurchaseReturns] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PurchaseReturns_Purchases_PurchaseId] FOREIGN KEY ([PurchaseId]) REFERENCES [Purchases] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730063025_AddSalesAndPurchaseReturns'
)
BEGIN
    CREATE TABLE [SaleReturns] (
        [Id] uniqueidentifier NOT NULL,
        [ReturnNumber] nvarchar(50) NOT NULL,
        [SaleId] uniqueidentifier NOT NULL,
        [ReturnDateUtc] datetime2 NOT NULL,
        [SubTotal] decimal(18,2) NOT NULL,
        [TaxAmount] decimal(18,2) NOT NULL,
        [DiscountAmount] decimal(18,2) NOT NULL,
        [TotalAmount] decimal(18,2) NOT NULL,
        [CustomerCreditApplied] decimal(18,2) NOT NULL,
        [RefundAmount] decimal(18,2) NOT NULL,
        [Reason] nvarchar(250) NOT NULL,
        [Notes] nvarchar(500) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_SaleReturns] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SaleReturns_Sales_SaleId] FOREIGN KEY ([SaleId]) REFERENCES [Sales] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730063025_AddSalesAndPurchaseReturns'
)
BEGIN
    CREATE TABLE [PurchaseReturnItems] (
        [Id] uniqueidentifier NOT NULL,
        [PurchaseReturnId] uniqueidentifier NOT NULL,
        [PurchaseItemId] uniqueidentifier NOT NULL,
        [ProductId] uniqueidentifier NOT NULL,
        [Quantity] decimal(18,2) NOT NULL,
        [UnitCost] decimal(18,2) NOT NULL,
        [TotalPrice] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_PurchaseReturnItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PurchaseReturnItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PurchaseReturnItems_PurchaseItems_PurchaseItemId] FOREIGN KEY ([PurchaseItemId]) REFERENCES [PurchaseItems] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PurchaseReturnItems_PurchaseReturns_PurchaseReturnId] FOREIGN KEY ([PurchaseReturnId]) REFERENCES [PurchaseReturns] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730063025_AddSalesAndPurchaseReturns'
)
BEGIN
    CREATE TABLE [SaleReturnItems] (
        [Id] uniqueidentifier NOT NULL,
        [SaleReturnId] uniqueidentifier NOT NULL,
        [SaleItemId] uniqueidentifier NOT NULL,
        [ProductId] uniqueidentifier NOT NULL,
        [Quantity] decimal(18,2) NOT NULL,
        [UnitPrice] decimal(18,2) NOT NULL,
        [TotalPrice] decimal(18,2) NOT NULL,
        [Restock] bit NOT NULL,
        CONSTRAINT [PK_SaleReturnItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SaleReturnItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_SaleReturnItems_SaleItems_SaleItemId] FOREIGN KEY ([SaleItemId]) REFERENCES [SaleItems] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_SaleReturnItems_SaleReturns_SaleReturnId] FOREIGN KEY ([SaleReturnId]) REFERENCES [SaleReturns] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730063025_AddSalesAndPurchaseReturns'
)
BEGIN
    CREATE INDEX [IX_PurchaseReturnItems_ProductId] ON [PurchaseReturnItems] ([ProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730063025_AddSalesAndPurchaseReturns'
)
BEGIN
    CREATE INDEX [IX_PurchaseReturnItems_PurchaseItemId] ON [PurchaseReturnItems] ([PurchaseItemId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730063025_AddSalesAndPurchaseReturns'
)
BEGIN
    CREATE INDEX [IX_PurchaseReturnItems_PurchaseReturnId] ON [PurchaseReturnItems] ([PurchaseReturnId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730063025_AddSalesAndPurchaseReturns'
)
BEGIN
    CREATE INDEX [IX_PurchaseReturns_PurchaseId] ON [PurchaseReturns] ([PurchaseId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730063025_AddSalesAndPurchaseReturns'
)
BEGIN
    CREATE UNIQUE INDEX [IX_PurchaseReturns_ReturnNumber] ON [PurchaseReturns] ([ReturnNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730063025_AddSalesAndPurchaseReturns'
)
BEGIN
    CREATE INDEX [IX_SaleReturnItems_ProductId] ON [SaleReturnItems] ([ProductId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730063025_AddSalesAndPurchaseReturns'
)
BEGIN
    CREATE INDEX [IX_SaleReturnItems_SaleItemId] ON [SaleReturnItems] ([SaleItemId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730063025_AddSalesAndPurchaseReturns'
)
BEGIN
    CREATE INDEX [IX_SaleReturnItems_SaleReturnId] ON [SaleReturnItems] ([SaleReturnId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730063025_AddSalesAndPurchaseReturns'
)
BEGIN
    CREATE UNIQUE INDEX [IX_SaleReturns_ReturnNumber] ON [SaleReturns] ([ReturnNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730063025_AddSalesAndPurchaseReturns'
)
BEGIN
    CREATE INDEX [IX_SaleReturns_SaleId] ON [SaleReturns] ([SaleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730063025_AddSalesAndPurchaseReturns'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260730063025_AddSalesAndPurchaseReturns', N'10.0.10');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730064224_AddDoubleEntryAccounting'
)
BEGIN
    CREATE TABLE [Accounts] (
        [Id] uniqueidentifier NOT NULL,
        [Code] nvarchar(20) NOT NULL,
        [Name] nvarchar(120) NOT NULL,
        [Type] int NOT NULL,
        [IsSystem] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Accounts] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730064224_AddDoubleEntryAccounting'
)
BEGIN
    CREATE TABLE [JournalEntries] (
        [Id] uniqueidentifier NOT NULL,
        [EntryNumber] nvarchar(50) NOT NULL,
        [EntryDateUtc] datetime2 NOT NULL,
        [Description] nvarchar(250) NOT NULL,
        [ReferenceType] nvarchar(50) NOT NULL,
        [ReferenceId] uniqueidentifier NULL,
        [IsAutomatic] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_JournalEntries] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730064224_AddDoubleEntryAccounting'
)
BEGIN
    CREATE TABLE [JournalLines] (
        [Id] uniqueidentifier NOT NULL,
        [JournalEntryId] uniqueidentifier NOT NULL,
        [AccountId] uniqueidentifier NOT NULL,
        [Debit] decimal(18,2) NOT NULL,
        [Credit] decimal(18,2) NOT NULL,
        [Memo] nvarchar(250) NULL,
        CONSTRAINT [PK_JournalLines] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_JournalLines_Accounts_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [Accounts] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_JournalLines_JournalEntries_JournalEntryId] FOREIGN KEY ([JournalEntryId]) REFERENCES [JournalEntries] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730064224_AddDoubleEntryAccounting'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Code', N'CreatedAtUtc', N'CreatedBy', N'DeletedAtUtc', N'DeletedBy', N'IsActive', N'IsDeleted', N'IsSystem', N'Name', N'Type', N'UpdatedAtUtc', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Accounts]'))
        SET IDENTITY_INSERT [Accounts] ON;
    EXEC(N'INSERT INTO [Accounts] ([Id], [Code], [CreatedAtUtc], [CreatedBy], [DeletedAtUtc], [DeletedBy], [IsActive], [IsDeleted], [IsSystem], [Name], [Type], [UpdatedAtUtc], [UpdatedBy])
    VALUES (''10000000-0000-0000-0000-000000000001'', N''1000'', ''2026-01-01T00:00:00.0000000Z'', N''System'', NULL, NULL, CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), N''Cash and Bank'', 1, NULL, NULL),
    (''10000000-0000-0000-0000-000000000002'', N''1100'', ''2026-01-01T00:00:00.0000000Z'', N''System'', NULL, NULL, CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), N''Accounts Receivable'', 1, NULL, NULL),
    (''10000000-0000-0000-0000-000000000003'', N''1200'', ''2026-01-01T00:00:00.0000000Z'', N''System'', NULL, NULL, CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), N''Inventory'', 1, NULL, NULL),
    (''20000000-0000-0000-0000-000000000001'', N''2000'', ''2026-01-01T00:00:00.0000000Z'', N''System'', NULL, NULL, CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), N''Accounts Payable'', 2, NULL, NULL),
    (''30000000-0000-0000-0000-000000000001'', N''3000'', ''2026-01-01T00:00:00.0000000Z'', N''System'', NULL, NULL, CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), N''Owner Equity'', 3, NULL, NULL),
    (''40000000-0000-0000-0000-000000000001'', N''4000'', ''2026-01-01T00:00:00.0000000Z'', N''System'', NULL, NULL, CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), N''Sales Revenue'', 4, NULL, NULL),
    (''40000000-0000-0000-0000-000000000002'', N''4100'', ''2026-01-01T00:00:00.0000000Z'', N''System'', NULL, NULL, CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), N''Sales Returns'', 4, NULL, NULL),
    (''50000000-0000-0000-0000-000000000001'', N''5000'', ''2026-01-01T00:00:00.0000000Z'', N''System'', NULL, NULL, CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), N''Cost of Goods Sold'', 5, NULL, NULL),
    (''50000000-0000-0000-0000-000000000002'', N''6000'', ''2026-01-01T00:00:00.0000000Z'', N''System'', NULL, NULL, CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), N''Operating Expenses'', 5, NULL, NULL),
    (''50000000-0000-0000-0000-000000000003'', N''5100'', ''2026-01-01T00:00:00.0000000Z'', N''System'', NULL, NULL, CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), N''Purchase Returns'', 5, NULL, NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Code', N'CreatedAtUtc', N'CreatedBy', N'DeletedAtUtc', N'DeletedBy', N'IsActive', N'IsDeleted', N'IsSystem', N'Name', N'Type', N'UpdatedAtUtc', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Accounts]'))
        SET IDENTITY_INSERT [Accounts] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730064224_AddDoubleEntryAccounting'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Accounts_Code] ON [Accounts] ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730064224_AddDoubleEntryAccounting'
)
BEGIN
    CREATE UNIQUE INDEX [IX_JournalEntries_EntryNumber] ON [JournalEntries] ([EntryNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730064224_AddDoubleEntryAccounting'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_JournalEntries_ReferenceType_ReferenceId] ON [JournalEntries] ([ReferenceType], [ReferenceId]) WHERE [ReferenceId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730064224_AddDoubleEntryAccounting'
)
BEGIN
    CREATE INDEX [IX_JournalLines_AccountId] ON [JournalLines] ([AccountId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730064224_AddDoubleEntryAccounting'
)
BEGIN
    CREATE INDEX [IX_JournalLines_JournalEntryId] ON [JournalLines] ([JournalEntryId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730064224_AddDoubleEntryAccounting'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260730064224_AddDoubleEntryAccounting', N'10.0.10');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730065452_AddProductMediaAndUniqueBarcodes'
)
BEGIN
    ALTER TABLE [Products] ADD [ImageUrl] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730065452_AddProductMediaAndUniqueBarcodes'
)
BEGIN
    UPDATE Products SET Barcode = NULL WHERE LTRIM(RTRIM(ISNULL(Barcode, ''))) = '';
    WITH DuplicateBarcodes AS
    (
        SELECT Id, ROW_NUMBER() OVER (PARTITION BY Barcode ORDER BY CreatedAtUtc, Id) AS RowNumber
        FROM Products
        WHERE Barcode IS NOT NULL
    )
    UPDATE Products
    SET Barcode = NULL
    WHERE Id IN (SELECT Id FROM DuplicateBarcodes WHERE RowNumber > 1);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730065452_AddProductMediaAndUniqueBarcodes'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Products_Barcode] ON [Products] ([Barcode]) WHERE [Barcode] IS NOT NULL AND [Barcode] <> ''''');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730065452_AddProductMediaAndUniqueBarcodes'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260730065452_AddProductMediaAndUniqueBarcodes', N'10.0.10');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730070145_AddPersistentNotifications'
)
BEGIN
    CREATE TABLE [UserNotifications] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [SourceKey] nvarchar(180) NOT NULL,
        [Type] nvarchar(20) NOT NULL,
        [Title] nvarchar(160) NOT NULL,
        [Message] nvarchar(500) NOT NULL,
        [Route] nvarchar(180) NULL,
        [IsRead] bit NOT NULL,
        [ReadAtUtc] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [UpdatedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_UserNotifications] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserNotifications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730070145_AddPersistentNotifications'
)
BEGIN
    CREATE INDEX [IX_UserNotifications_UserId_IsActive_IsRead] ON [UserNotifications] ([UserId], [IsActive], [IsRead]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730070145_AddPersistentNotifications'
)
BEGIN
    CREATE UNIQUE INDEX [IX_UserNotifications_UserId_SourceKey] ON [UserNotifications] ([UserId], [SourceKey]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730070145_AddPersistentNotifications'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260730070145_AddPersistentNotifications', N'10.0.10');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730070749_AddSessionDeviceMetadata'
)
BEGIN
    ALTER TABLE [RefreshTokens] ADD [UserAgent] nvarchar(300) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260730070749_AddSessionDeviceMetadata'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260730070749_AddSessionDeviceMetadata', N'10.0.10');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731091917_AddPersistentEmployees'
)
BEGIN
    CREATE TABLE [Employees] (
        [Id] uniqueidentifier NOT NULL,
        [EmployeeCode] nvarchar(30) NOT NULL,
        [FirstName] nvarchar(80) NOT NULL,
        [LastName] nvarchar(80) NOT NULL,
        [Email] nvarchar(150) NOT NULL,
        [Phone] nvarchar(30) NOT NULL,
        [Department] nvarchar(100) NOT NULL,
        [Designation] nvarchar(120) NOT NULL,
        [HireDateUtc] datetime2 NOT NULL,
        [MonthlySalary] decimal(18,2) NOT NULL,
        [Status] nvarchar(30) NOT NULL,
        [AvatarColor] nvarchar(20) NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAtUtc] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Employees] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731091917_AddPersistentEmployees'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Employees_Email] ON [Employees] ([Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731091917_AddPersistentEmployees'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Employees_EmployeeCode] ON [Employees] ([EmployeeCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731091917_AddPersistentEmployees'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260731091917_AddPersistentEmployees', N'10.0.10');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731095317_AddPersistentTeamChat'
)
BEGIN
    CREATE TABLE [TeamChatMessages] (
        [Id] uniqueidentifier NOT NULL,
        [TargetEmployeeId] uniqueidentifier NOT NULL,
        [SenderUserId] uniqueidentifier NULL,
        [SenderName] nvarchar(160) NOT NULL,
        [SenderRole] nvarchar(80) NOT NULL,
        [AvatarColor] nvarchar(20) NOT NULL,
        [Content] nvarchar(4000) NOT NULL,
        [TimestampUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_TeamChatMessages] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731095317_AddPersistentTeamChat'
)
BEGIN
    CREATE INDEX [IX_TeamChatMessages_TargetEmployeeId_TimestampUtc] ON [TeamChatMessages] ([TargetEmployeeId], [TimestampUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731095317_AddPersistentTeamChat'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260731095317_AddPersistentTeamChat', N'10.0.10');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731095624_AddSubscriptionBillingTables'
)
BEGIN
    CREATE TABLE [SubscriptionPlans] (
        [Id] uniqueidentifier NOT NULL PRIMARY KEY, [Name] nvarchar(100) NOT NULL,
        [Code] nvarchar(50) NOT NULL, [MonthlyPrice] decimal(18,2) NOT NULL,
        [AnnualPrice] decimal(18,2) NOT NULL, [MaxUsers] int NOT NULL,
        [MaxSalesPerMonth] int NOT NULL, [EnableAiAssistant] bit NOT NULL,
        [EnableDoubleEntryAccounting] bit NOT NULL, [EnablePdfGeneration] bit NOT NULL,
        [IsActive] bit NOT NULL, [Description] nvarchar(max) NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL, [CreatedBy] nvarchar(max) NULL,
        [UpdatedAtUtc] datetime2 NULL, [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL, [DeletedAtUtc] datetime2 NULL, [DeletedBy] nvarchar(max) NULL);

    CREATE TABLE [TenantSubscriptions] (
        [Id] uniqueidentifier NOT NULL PRIMARY KEY, [PlanId] uniqueidentifier NOT NULL,
        [Status] int NOT NULL, [StartDateUtc] datetime2 NOT NULL, [EndDateUtc] datetime2 NULL,
        [TrialEndsAtUtc] datetime2 NULL, [AutoRenew] bit NOT NULL,
        [PaymentMethod] nvarchar(100) NOT NULL, [ExternalCustomerId] nvarchar(100) NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL, [CreatedBy] nvarchar(max) NULL,
        [UpdatedAtUtc] datetime2 NULL, [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL, [DeletedAtUtc] datetime2 NULL, [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [FK_TenantSubscriptions_SubscriptionPlans_PlanId]
            FOREIGN KEY ([PlanId]) REFERENCES [SubscriptionPlans] ([Id]) ON DELETE CASCADE);
    CREATE INDEX [IX_TenantSubscriptions_PlanId] ON [TenantSubscriptions] ([PlanId]);

    CREATE TABLE [SubscriptionInvoices] (
        [Id] uniqueidentifier NOT NULL PRIMARY KEY, [SubscriptionId] uniqueidentifier NOT NULL,
        [InvoiceNumber] nvarchar(50) NOT NULL, [Amount] decimal(18,2) NOT NULL,
        [Currency] nvarchar(10) NOT NULL, [Status] int NOT NULL,
        [BillingDateUtc] datetime2 NOT NULL, [PaymentTransactionReference] nvarchar(100) NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL, [CreatedBy] nvarchar(max) NULL,
        [UpdatedAtUtc] datetime2 NULL, [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL, [DeletedAtUtc] datetime2 NULL, [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [FK_SubscriptionInvoices_TenantSubscriptions_SubscriptionId]
            FOREIGN KEY ([SubscriptionId]) REFERENCES [TenantSubscriptions] ([Id]) ON DELETE CASCADE);
    CREATE INDEX [IX_SubscriptionInvoices_SubscriptionId] ON [SubscriptionInvoices] ([SubscriptionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260731095624_AddSubscriptionBillingTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260731095624_AddSubscriptionBillingTables', N'10.0.10');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [Users] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [UserNotifications] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [TenantSubscriptions] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [TeamChatMessages] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [Suppliers] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [SubscriptionPlans] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [SubscriptionInvoices] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [Sales] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [SaleReturns] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [SaleReturnItems] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [SaleItems] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [RefreshTokens] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [Purchases] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [PurchaseReturns] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [PurchaseReturnItems] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [PurchaseItems] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [ProductUnits] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [Products] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [Payments] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [JournalLines] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [JournalEntries] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [InventoryMovements] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [InventoryAdjustments] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [Expenses] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [ExpenseCategories] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [Employees] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [Customers] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [Categories] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [BusinessProfiles] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [AuditLogs] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [AiChatMessages] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [Accounts] ADD [TenantId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    CREATE TABLE [Tenants] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Slug] nvarchar(100) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_Tenants] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    DECLARE @LegacyTenant uniqueidentifier = '11111111-1111-1111-1111-111111111111';
    INSERT INTO Tenants (Id, Name, Slug, IsActive, CreatedAtUtc)
    VALUES (@LegacyTenant, 'Legacy Enterprise', 'legacy-enterprise', 1, SYSUTCDATETIME());

    UPDATE Users SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
    UPDATE UserNotifications SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
    UPDATE TenantSubscriptions SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
    UPDATE TeamChatMessages SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
    UPDATE Suppliers SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
    UPDATE SubscriptionInvoices SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
    UPDATE Sales SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
    UPDATE SaleReturns SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
    UPDATE SaleReturnItems SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
    UPDATE SaleItems SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
    UPDATE Purchases SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
    UPDATE PurchaseReturns SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
    UPDATE PurchaseReturnItems SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
    UPDATE PurchaseItems SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
    UPDATE ProductUnits SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
    UPDATE Products SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
    UPDATE Payments SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
    UPDATE JournalLines SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
    UPDATE JournalEntries SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
    UPDATE InventoryMovements SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
    UPDATE InventoryAdjustments SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
    UPDATE Expenses SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
    UPDATE ExpenseCategories SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
    UPDATE Employees SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
    UPDATE Customers SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
    UPDATE Categories SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
    UPDATE BusinessProfiles SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
    UPDATE AuditLogs SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
    UPDATE AiChatMessages SET TenantId = @LegacyTenant WHERE TenantId = '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    EXEC(N'UPDATE [Accounts] SET [TenantId] = ''00000000-0000-0000-0000-000000000000''
    WHERE [Id] = ''10000000-0000-0000-0000-000000000001'';
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    EXEC(N'UPDATE [Accounts] SET [TenantId] = ''00000000-0000-0000-0000-000000000000''
    WHERE [Id] = ''10000000-0000-0000-0000-000000000002'';
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    EXEC(N'UPDATE [Accounts] SET [TenantId] = ''00000000-0000-0000-0000-000000000000''
    WHERE [Id] = ''10000000-0000-0000-0000-000000000003'';
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    EXEC(N'UPDATE [Accounts] SET [TenantId] = ''00000000-0000-0000-0000-000000000000''
    WHERE [Id] = ''20000000-0000-0000-0000-000000000001'';
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    EXEC(N'UPDATE [Accounts] SET [TenantId] = ''00000000-0000-0000-0000-000000000000''
    WHERE [Id] = ''30000000-0000-0000-0000-000000000001'';
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    EXEC(N'UPDATE [Accounts] SET [TenantId] = ''00000000-0000-0000-0000-000000000000''
    WHERE [Id] = ''40000000-0000-0000-0000-000000000001'';
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    EXEC(N'UPDATE [Accounts] SET [TenantId] = ''00000000-0000-0000-0000-000000000000''
    WHERE [Id] = ''40000000-0000-0000-0000-000000000002'';
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    EXEC(N'UPDATE [Accounts] SET [TenantId] = ''00000000-0000-0000-0000-000000000000''
    WHERE [Id] = ''50000000-0000-0000-0000-000000000001'';
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    EXEC(N'UPDATE [Accounts] SET [TenantId] = ''00000000-0000-0000-0000-000000000000''
    WHERE [Id] = ''50000000-0000-0000-0000-000000000002'';
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    EXEC(N'UPDATE [Accounts] SET [TenantId] = ''00000000-0000-0000-0000-000000000000''
    WHERE [Id] = ''50000000-0000-0000-0000-000000000003'';
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    CREATE INDEX [IX_Users_TenantId] ON [Users] ([TenantId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Tenants_Slug] ON [Tenants] ([Slug]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    ALTER TABLE [Users] ADD CONSTRAINT [FK_Users_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Tenants] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803052957_AddMultiTenantIsolation'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260803052957_AddMultiTenantIsolation', N'10.0.10');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    DROP INDEX [IX_Suppliers_SupplierCode] ON [Suppliers];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    DROP INDEX [IX_Sales_InvoiceNumber] ON [Sales];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    DROP INDEX [IX_SaleReturns_ReturnNumber] ON [SaleReturns];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    DROP INDEX [IX_Purchases_PurchaseOrderNumber] ON [Purchases];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    DROP INDEX [IX_PurchaseReturns_ReturnNumber] ON [PurchaseReturns];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    DROP INDEX [IX_Products_Barcode] ON [Products];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    DROP INDEX [IX_Products_SKU] ON [Products];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    DROP INDEX [IX_Payments_PaymentNumber] ON [Payments];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    DROP INDEX [IX_JournalEntries_EntryNumber] ON [JournalEntries];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    DROP INDEX [IX_JournalEntries_ReferenceType_ReferenceId] ON [JournalEntries];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    DROP INDEX [IX_InventoryMovements_MovementNumber] ON [InventoryMovements];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    DROP INDEX [IX_InventoryAdjustments_AdjustmentNumber] ON [InventoryAdjustments];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    DROP INDEX [IX_Expenses_ExpenseNumber] ON [Expenses];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    DROP INDEX [IX_Employees_Email] ON [Employees];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    DROP INDEX [IX_Employees_EmployeeCode] ON [Employees];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    DROP INDEX [IX_Customers_CustomerCode] ON [Customers];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Suppliers_TenantId_SupplierCode] ON [Suppliers] ([TenantId], [SupplierCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Sales_TenantId_InvoiceNumber] ON [Sales] ([TenantId], [InvoiceNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    CREATE UNIQUE INDEX [IX_SaleReturns_TenantId_ReturnNumber] ON [SaleReturns] ([TenantId], [ReturnNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Purchases_TenantId_PurchaseOrderNumber] ON [Purchases] ([TenantId], [PurchaseOrderNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    CREATE UNIQUE INDEX [IX_PurchaseReturns_TenantId_ReturnNumber] ON [PurchaseReturns] ([TenantId], [ReturnNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Products_TenantId_Barcode] ON [Products] ([TenantId], [Barcode]) WHERE [Barcode] IS NOT NULL AND [Barcode] <> ''''');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Products_TenantId_SKU] ON [Products] ([TenantId], [SKU]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Payments_TenantId_PaymentNumber] ON [Payments] ([TenantId], [PaymentNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    CREATE UNIQUE INDEX [IX_JournalEntries_TenantId_EntryNumber] ON [JournalEntries] ([TenantId], [EntryNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_JournalEntries_TenantId_ReferenceType_ReferenceId] ON [JournalEntries] ([TenantId], [ReferenceType], [ReferenceId]) WHERE [ReferenceId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    CREATE UNIQUE INDEX [IX_InventoryMovements_TenantId_MovementNumber] ON [InventoryMovements] ([TenantId], [MovementNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    CREATE UNIQUE INDEX [IX_InventoryAdjustments_TenantId_AdjustmentNumber] ON [InventoryAdjustments] ([TenantId], [AdjustmentNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Expenses_TenantId_ExpenseNumber] ON [Expenses] ([TenantId], [ExpenseNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Employees_TenantId_Email] ON [Employees] ([TenantId], [Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Employees_TenantId_EmployeeCode] ON [Employees] ([TenantId], [EmployeeCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Customers_TenantId_CustomerCode] ON [Customers] ([TenantId], [CustomerCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260803053258_AddTenantScopedUniqueIndexes'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260803053258_AddTenantScopedUniqueIndexes', N'10.0.10');
END;

COMMIT;
GO

