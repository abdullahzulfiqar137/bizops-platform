# BizOps production deployment

## Container deployment

The repository includes a production Compose stack for SQL Server, the API, and the Angular/Nginx frontend.

```powershell
Copy-Item .env.production.example .env.production
# Replace every example value in .env.production before continuing.
docker compose --env-file .env.production -f docker-compose.production.yml config
docker compose --env-file .env.production -f docker-compose.production.yml up --build -d
```

The database, uploads, and backups use named persistent volumes. Back up those volumes independently of the host. TLS must terminate at a trusted ingress or load balancer in front of the published web port. Only the web service is exposed publicly; the database network is internal.

Production startup intentionally fails when the database connection is missing or points to LocalDB/a developer workstation, when JWT URLs or CORS origins are not HTTPS, or when placeholder/loopback hosts are configured.

## Required secrets

Configure these in the hosting platform; never commit their values:

- `ConnectionStrings__DefaultConnection`
- `JwtSettings__SecretKey` (minimum 32 random characters)
- `JwtSettings__Issuer`
- `JwtSettings__Audience`
- `AllowedOrigins__0`
- `Tenancy__PinnedTenantId` only for a dedicated single-tenant enterprise deployment
- `Email__Host`
- `Email__Port`
- `Email__Username`
- `Email__Password`
- `Email__FromAddress`
- `Email__FrontendBaseUrl`
- `DatabaseBackups__Directory`
- `DatabaseBackups__ScheduleEnabled=true`
- `DatabaseBackups__ScheduleIntervalHours=24`
- `DatabaseBackups__OffsiteDirectory` (a separate mounted disk/share)

The Angular production build calls `/api` on the same origin. Configure the web
server or gateway to proxy `/api` to the ASP.NET Core API.

## Tenancy modes

- SaaS mode: leave `Tenancy__PinnedTenantId` empty. Registration provisions a new
  tenant and makes the registering user that tenant's administrator.
- Dedicated enterprise mode: set `Tenancy__PinnedTenantId` to the tenant UUID assigned
  to that enterprise. A migrated pre-SaaS installation is assigned
  `11111111-1111-1111-1111-111111111111`.
- Never reuse one pinned tenant ID for independent customers. Separate enterprise
  deployments must use separate databases and tenant IDs.
- Refresh tokens are issued as HttpOnly cookies. The gateway must preserve `Set-Cookie`,
  HTTPS, and credentialed requests to `/api`.

## Release checklist

1. Run the GitHub Actions `BizOps CI` workflow.
2. Download the validated frontend and API artifacts.
3. Configure all required secrets and an HTTPS certificate.
4. Review and apply the idempotent `artifacts/database-migrations.sql` during a maintenance window.
5. Deploy the API, then the frontend.
6. Verify `/api/health`, login and MFA, active sessions, password recovery email,
   tenant isolation, permissions, currency, invoice printing, returns, imports, exports, alerts,
   accounting synchronization, primary backup creation, and off-device backup copy.
7. Keep database restore disabled outside an approved recovery window.
8. Configure monitoring to alert on a non-200 `/api/health`, low free storage, stale
   backups, and repeated authentication or scheduled-backup errors in structured logs.
9. Keep subscription upgrades disabled until a payment provider webhook has been
   implemented and cryptographically verified. The API intentionally returns HTTP 501
   instead of fabricating a successful charge.
