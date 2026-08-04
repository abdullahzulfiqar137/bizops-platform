# BizOps - Business Operations Platform

BizOps is a full-stack enterprise resource planning application that brings daily operations, financial records, inventory, reporting, security, and administration into one responsive workspace.

> Status: Production-level application baseline | Version: 1.0

## Core features

- JWT authentication, rotating refresh tokens, password recovery, account lockout, authenticator MFA, and role-based access
- Roles for administrators, managers, and staff
- Customer and supplier management with live balances and database-backed transaction/purchase histories
- Product catalog, images, unique barcodes, CSV imports, categories, managed measurement units, inventory adjustments, and stock movements
- Transactional sales invoices, purchase orders, partial/full returns, refunds, validated payments, and operational expenses
- Dashboard, financial reports, CSV exports, and printable PDF documents
- Configurable company profile, currency, tax rate, and invoice numbering
- Read-only audit trail showing who changed each record and when
- AI business assistant with stored conversation history
- Persistent low-stock and overdue-balance alerts with per-user read state
- Double-entry journals, chart of accounts, ledgers, trial balance, and balance sheet
- Administrator database backup, scheduled retention, off-device copy, download, and guarded restore tools

## Business modules

| Module | What it manages |
| --- | --- |
| Dashboard | Business KPIs, revenue, expenses, receivables, payables, and stock alerts |
| Sales and billing | Customer invoices, discounts, tax, balances, cancellation, printing, and PDF output |
| Purchases | Supplier purchase orders, payable balances, stock receipts, and cancellation |
| Payments | Customer receipts, supplier settlements, payment methods, and references |
| Expenses | Operational expenses and expense categories |
| Products and inventory | Products, categories, SKUs, stock levels, adjustments, and movement history |
| Business partners | Customer and supplier profiles with financial balances |
| Reports | Profit and loss, sales performance, expense distribution, and CSV export |
| Accounting | Automatic/manual journals, chart of accounts, general ledger, trial balance, and balance sheet |
| Returns | Partial or complete customer/supplier returns, stock reversal, credits, and refunds |
| Administration | Users, roles, permissions, audit logs, company settings, and database backups |
| AI assistant | Platform guidance and contextual business assistance with conversation history |

## Implemented data integrity

- Sales, purchases, cancellations, returns, and payments update related documents, inventory movements, stock quantities, and partner balances atomically.
- A failed multi-record operation is rolled back instead of leaving partially updated inventory or financial data.
- Customer history is generated from real sales, customer payments, and sales returns, including the running balance after each transaction.
- Supplier history is generated from real purchase orders with paid amount, remaining balance, and payment status.
- Payments cannot exceed the applicable outstanding balance.
- A customer payment cannot be applied to another customer's invoice, and a supplier payment cannot be applied to another supplier's purchase.
- Product-unit names and abbreviations are unique within a tenant. Units referenced by products cannot be deleted.
- Tenant query filters and tenant-scoped indexes isolate business records between tenants.

## Product unit API

The frontend and API support listing and managing product measurement units:

| Method | Endpoint | Access |
| --- | --- | --- |
| `GET` | `/api/product-units` | Authenticated users |
| `GET` | `/api/product-units/{id}` | Authenticated users |
| `POST` | `/api/product-units` | Admin or Manager |
| `PUT` | `/api/product-units/{id}` | Admin or Manager |
| `DELETE` | `/api/product-units/{id}` | Admin or Manager |

## Technology

| Area | Stack |
| --- | --- |
| Frontend | Angular 20, TypeScript, RxJS, SCSS |
| Backend | .NET 10, ASP.NET Core Web API, Entity Framework Core |
| Database | Microsoft SQL Server |
| Security | ASP.NET Core Identity, JWT bearer authentication, rate limiting |
| Validation and logging | FluentValidation, Serilog |

## Repository structure

```text
Project OS/
|-- frontend/business-operations-ui/   Angular application
|-- backend/BusinessOperations.API/    ASP.NET Core API
|-- backend/BusinessOperations.API.Tests/
|-- backend/BusinessOperations.sln
|-- DEPLOYMENT.md
`-- README.md
```

## Requirements

- .NET 10 SDK
- Node.js 20 or newer and npm
- SQL Server, SQL Server Express, or LocalDB
- Google Chrome for frontend browser tests

## Local setup

### 1. Configure the database

Set `ConnectionStrings:DefaultConnection` in:

```text
backend/BusinessOperations.API/appsettings.Development.json
```

Apply the migrations:

```powershell
cd "D:\Project OS\backend\BusinessOperations.API"
dotnet ef database update
```

For controlled production releases, an idempotent SQL script is also generated at
`artifacts/database-migrations.sql`. Review it and run it with the deployment
database credentials during the maintenance window.

If `dotnet ef` is unavailable, install the matching EF Core CLI tool:

```powershell
dotnet tool install --global dotnet-ef --version 10.*
```

### 2. Start the backend

```powershell
cd "D:\Project OS\backend\BusinessOperations.API"
dotnet run --urls http://localhost:5000
```

The API and Swagger documentation are available at:

```text
http://localhost:5000/swagger
```

### 3. Start the frontend

```powershell
cd "D:\Project OS\frontend\business-operations-ui"
npm install
npm start
```

Open:

```text
http://localhost:4200
```

Angular watch mode and polling are enabled, so saved frontend changes should rebuild and reload without restarting the server.

## Local URLs

| Service | Address |
| --- | --- |
| Frontend | `http://localhost:4200` |
| Backend API | `http://localhost:5000/api` |
| Swagger | `http://localhost:5000/swagger` |

## Development login

The development configuration can create this local administrator:

```text
Username: admin
Password: Admin123!
Role: Admin
```

These credentials are for local development only. Production has no built-in administrator password. Supply the initial administrator through secure environment variables:

```text
InitialAdmin__Email
InitialAdmin__Username
InitialAdmin__Password
Tenancy__PinnedTenantId
```

Remove those values after the first production administrator has been created.

## Configuration

Development settings are stored in `backend/BusinessOperations.API/appsettings.Development.json`. Production secrets must be supplied by the hosting platform and must never be committed.

Important production variables:

```text
ConnectionStrings__DefaultConnection
JwtSettings__SecretKey
JwtSettings__Issuer
JwtSettings__Audience
AllowedOrigins__0
Email__Host
Email__Port
Email__Username
Email__Password
Email__FromAddress
Email__FrontendBaseUrl
DatabaseBackups__Directory
DatabaseBackups__ScheduleEnabled
DatabaseBackups__ScheduleIntervalHours
DatabaseBackups__OffsiteDirectory
InitialAdmin__Email
InitialAdmin__Username
InitialAdmin__Password
Tenancy__PinnedTenantId
```

The Angular development build calls `http://localhost:5000/api`. The production build calls `/api`, so the production web server must proxy `/api` to the backend.

## Authentication flow

1. The user submits credentials on the login page.
2. The API validates the account, password, role, lockout state, and rate limit.
3. If MFA is enabled, the user enters the current authenticator code.
4. A short-lived JWT access token and rotating refresh token are issued.
5. The frontend attaches the access token to protected API requests.
6. The refresh token obtains a new token pair when required.
7. The Account Security page can revoke individual devices or all refresh sessions.

Use the browser's **Developer Tools > Network > login request > Response** to confirm token generation. Avoid sharing or copying production tokens.

## Verification

Current verified baseline:

- Backend: 20 tests passing
- Frontend: 3 tests passing in headless Chrome
- .NET Release build: zero warnings and zero errors
- Angular production build and TypeScript check: passing
- EF Core: no pending model changes after the latest migration

Run the frontend checks:

```powershell
cd "D:\Project OS\frontend\business-operations-ui"
npx tsc -p tsconfig.app.json --noEmit
npm run test:ci
npm run build -- --configuration production
```

Run the backend checks:

```powershell
cd "D:\Project OS\backend"
dotnet test BusinessOperations.sln --configuration Release
dotnet build BusinessOperations.sln --configuration Release
```

## Production and recovery

- See [DEPLOYMENT.md](DEPLOYMENT.md) for production configuration and deployment.
- A production Docker Compose stack and environment template are provided at the repository root. Production startup fails fast when required security and database settings are unsafe or missing.
- See [backend/DATABASE-RECOVERY.md](backend/DATABASE-RECOVERY.md) for backup and restore guidance.
- Database restore is intentionally disabled by default and must be explicitly enabled in trusted environments.
- Store JWT secrets, SMTP credentials, database passwords, and initial-admin values outside source control.
- `DatabaseBackups__OffsiteDirectory` must point to a separate mounted disk, network share, or cloud-synced volume—not the primary backup folder.
- `/api/health` reports database connectivity, uptime, free storage, and backup freshness.

## Subscription billing boundary

Subscription plans, current subscription state, usage, and invoices are stored in SQL Server and exposed under `/api/subscriptions`. The frontend does not treat a rejected payment as successful and does not create fake paid invoices as a fallback.

Real card or wallet charging requires an external PCI-compliant payment provider and verified webhook. Until a provider is configured, `/api/subscriptions/subscribe` returns `501 Not Implemented` and leaves the subscription and invoices unchanged. Raw card numbers and security codes must never be stored in this application database.

## Data import templates

Customer, supplier, and product pages include an **Import CSV** action for Admin and Manager users. Download the template from the import dialog, keep its header names unchanged, and review row-level errors after upload. Product category and unit values must already exist.

## Operational reminders

The topbar alert center stores notifications per user. Alerts are generated for active products at or below minimum stock and sales or purchases with balances outstanding for more than 30 days. Resolved conditions automatically disappear, while read state remains user-specific.

## Common development issues

### Frontend changes do not reload

Keep `npm start` running. It uses Angular watch mode with polling. Save the file and wait for:

```text
Application bundle generation complete.
Page reload sent to client(s).
```

If compilation fails, fix the first displayed error; Angular will rebuild automatically after the next save.

### Frontend cannot reach the API

Confirm that the backend is running on port `5000`, then inspect the failed request under **Developer Tools > Network**. A `401` response means authentication is missing or expired; a connection error means the API is not reachable.

### LocalDB cannot create an automatic instance

The application migrations are still valid, but the local SQL runtime needs repair or a reachable SQL Server connection. Start/repair `MSSQLLocalDB`, or set `ConnectionStrings__DefaultConnection` to your running SQL Server instance, then run `dotnet ef database update`. Do not delete an existing LocalDB instance before backing up its databases.

### Forgot-password email is not received

Configure the SMTP-related `Email__*` variables. The endpoint intentionally returns a generic response even when an email address is unknown, which prevents account discovery.

## License

Proprietary software for internal business operations management. All rights reserved.
