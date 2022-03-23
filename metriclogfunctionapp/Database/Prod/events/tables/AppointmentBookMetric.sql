
CREATE TABLE IF NOT EXISTS events."AppointmentBookMetric" (
    "Timestamp" timestamp with time zone NOT NULL,
    "SessionId" character varying NOT NULL
);

CALL perms.apply_etl_table_permissions('events', 'AppointmentBookMetric');

CREATE INDEX IF NOT EXISTS AppointmentBookMetric_date_idx on events."AppointmentBookMetric" ("Timestamp");
