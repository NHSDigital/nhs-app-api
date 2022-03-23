
CREATE TABLE IF NOT EXISTS events."PrescriptionOrdersMetric" (
    "Timestamp" timestamp with time zone NOT NULL,
    "SessionId" character varying(36) NOT NULL
);

CALL perms.apply_etl_table_permissions('events', 'PrescriptionOrdersMetric');

CREATE INDEX IF NOT EXISTS PrescriptionOrdersMetric_SessionId_idx on events."PrescriptionOrdersMetric" ("SessionId");
CREATE INDEX IF NOT EXISTS PrescriptionOrdersMetric_date_idx on events."PrescriptionOrdersMetric" ("Timestamp");
