
CREATE TABLE IF NOT EXISTS events."AppointmentCancelMetric" (
    "Timestamp" timestamp with time zone NOT NULL,
    "SessionId" character varying (36) NULL,
    "AuditId" character varying (36) NULL
);

ALTER TABLE events."AppointmentCancelMetric"
    DROP CONSTRAINT IF EXISTS appointmentcancelmetric_auditid_unique,
    ADD CONSTRAINT appointmentcancelmetric_auditid_unique UNIQUE ("AuditId");

CALL perms.apply_etl_table_permissions('events', 'AppointmentCancelMetric');

CREATE INDEX IF NOT EXISTS AppointmentCancelMetric_date_idx on events."AppointmentCancelMetric" ("Timestamp");
