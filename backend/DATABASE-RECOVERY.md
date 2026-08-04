# Database backup and recovery

The Backup Center is restricted to the `Admin` role. Backups are written to
`DatabaseBackups:Directory`, retained according to `RetentionDays`, and excluded
from source control.

## Production configuration

Set these through environment variables or the deployment secret manager:

- `ConnectionStrings__DefaultConnection`
- `JwtSettings__SecretKey`
- `Email__Host`, `Email__Username`, `Email__Password`, `Email__FromAddress`
- `DatabaseBackups__Directory` to a protected volume available to SQL Server

The SQL Server service account must have write/read permission for the backup
directory. Store an additional encrypted copy outside the application server.

## Safe restore procedure

1. Announce a maintenance window and stop all user activity.
2. Verify the selected `.bak` file and retain a copy of the current database.
3. Temporarily set `DatabaseBackups__EnableRestore=true`.
4. Restart the API, sign in as an Admin, and open Database Backup Center.
5. Enter the exact confirmation phrase displayed by the page.
6. After a successful restore, immediately set restore back to `false`.
7. Restart the API and verify health, login, record counts, and recent reports.

Restore is intentionally disabled by default and should never remain enabled
during normal operation.
