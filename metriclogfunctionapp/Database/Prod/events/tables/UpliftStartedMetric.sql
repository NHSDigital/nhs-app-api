CREATE TABLE IF NOT EXISTS events."UpliftStartedMetric" (
    "Timestamp" timestamp with time zone NOT NULL,
    "SessionId" character varying NOT NULL
);

CALL perms.apply_etl_table_permissions('events', 'UpliftStartedMetric');

CREATE INDEX IF NOT EXISTS UpliftStartedMetric_SessionId_idx on events."UpliftStartedMetric" ("SessionId");
CREATE INDEX IF NOT EXISTS UpliftStartedMetric_date_idx on events."UpliftStartedMetric" ("Timestamp");
