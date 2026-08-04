export interface AuditLogResponse {
  id: string;
  userId: string | null;
  userName: string | null;
  action: string;
  entityName: string;
  entityId: string;
  oldValues: string | null;
  newValues: string | null;
  affectedColumns: string | null;
  ipAddress: string | null;
  userAgent: string | null;
  timestampUtc: string;
}
