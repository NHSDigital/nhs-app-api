
CREATE TABLE IF NOT EXISTS events."AppointmentCancelMetric" (
    "Timestamp" timestamp with time zone NOT NULL,
    "SessionId" character varying NOT NULL
);

CALL perms.apply_etl_table_permissions('events', 'AppointmentCancelMetric');

CREATE INDEX IF NOT EXISTS AppointmentCancelMetric_date_idx on events."AppointmentCancelMetric" ("Timestamp");
