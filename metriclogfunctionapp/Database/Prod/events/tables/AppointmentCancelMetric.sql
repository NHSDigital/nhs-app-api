
CREATE TABLE IF NOT EXISTS events."AppointmentCancelMetric" (
    "Timestamp" timestamp with time zone NOT NULL,
    "SessionId" character varying (36) NULL,
    "AuditId" character varying (36) NULL
);

CALL perms.apply_etl_table_permissions('events', 'AppointmentCancelMetric');
