# BizOps — Business Operations Platform

A full-stack business operations and ERP-style platform for managing sales, purchases, inventory, financial records, reporting, and administration in one workspace.

> **Status:** Active development baseline. Feature availability and deployment configuration should be verified against the current source before production use.

## Highlights

- JWT authentication, refresh-token rotation, password recovery, MFA, account lockout, and role-based access
- Administrator, manager, and staff roles
- Customer and supplier management with transaction histories and balances
- Product catalog, barcode support, CSV imports, stock adjustments, and inventory movements
- Sales invoices, purchase orders, returns, refunds, payments, and expenses
- Dashboards, financial reports, CSV exports, and printable PDF documents
- Company settings, configurable currency, tax, and invoice numbering
- Audit trail and tenant-scoped business data
- AI assistant with stored conversation history
- Accounting workflows including journals, chart of accounts, ledgers, trial balance, and balance sheet
- Database backup and guarded restore workflows

## Technology stack

| Layer | Technology |
| --- | --- |
| Frontend | Angular 20, TypeScript, RxJS, SCSS |
| Backend | .NET 10, ASP.NET Core Web API |
| Data access | Entity Framework Core |
| Database | Microsoft SQL Server |
| Security | ASP.NET Core Identity, JWT bearer authentication |
| Validation and logging | FluentValidation, Serilog |

## Repository layout

```text
.
├── frontend/
│   └── business-operations-ui/       # Angular application
├── backend/
│   ├── BusinessOperations.API/       # ASP.NET Core API
│   ├── BusinessOperations.API.Tests/ # Backend tests
│   └── BusinessOperations.sln
├── artifacts/
├── DEPLOYMENT.md
└── README.md
```

## Prerequisites

- .NET 10 SDK
- Node.js and npm (use a version supported by the Angular workspace)
- SQL Server, SQL Server Express, or LocalDB

## Local development

### 1. Configure SQL Server

Set `ConnectionStrings:DefaultConnection` in:

```text
backend/BusinessOperations.API/appsettings.Development.json
```

Apply database migrations:

```powershell
cd backend/BusinessOperations.API
dotnet ef database update
```

If the EF Core CLI is not installed:

```powershell
dotnet tool install --global dotnet-ef --version 10.*
```

### 2. Run the API

```powershell
cd backend/BusinessOperations.API
dotnet run --urls http://localhost:5000
```

Swagger: http://localhost:5000/swagger

### 3. Run the Angular application

```powershell
cd frontend/business-operations-ui
npm install
npm start
```

Frontend: http://localhost:4200

The Angular development build is configured to call the local API. For production, the web server must route `/api` requests to the backend as described in [DEPLOYMENT.md](DEPLOYMENT.md).

## Configuration and secrets

Keep database passwords, JWT signing keys, SMTP credentials, administrator bootstrap passwords, and other secrets outside source control. Use environment variables or your hosting platform's secret manager for production configuration.

The application README previously listed a default development administrator password. That password has intentionally been removed from this public documentation. Inspect the local development seed/configuration to understand how a local account is provisioned; use a unique local-only password and never reuse it in a deployed environment. If any published sample credential was ever used on a reachable system, rotate it immediately.

See [DEPLOYMENT.md](DEPLOYMENT.md) for production configuration and deployment guidance.

## Verification

The project documentation has previously reported backend/frontend test and build results. Re-run checks against the current checkout before relying on those results:

```powershell
cd backend
dotnet test BusinessOperations.sln --configuration Release
dotnet build BusinessOperations.sln --configuration Release
```

```powershell
cd frontend/business-operations-ui
npm install
npx tsc -p tsconfig.app.json --noEmit
npm run test:ci
npm run build -- --configuration production
```

## Further documentation

- [Deployment guide](DEPLOYMENT.md)
- [Database recovery guide](backend/DATABASE-RECOVERY.md)

## License

The existing project documentation describes this software as proprietary. No open-source license is granted by this README; please contact the repository owner for usage permissions.
