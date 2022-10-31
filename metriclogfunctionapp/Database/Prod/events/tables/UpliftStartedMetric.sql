CREATE TABLE IF NOT EXISTS events."UpliftStartedMetric" (
    "Timestamp" timestamp with time zone NOT NULL,
    "SessionId" character varying NOT NULL
);

CALL perms.apply_etl_table_permissions('events', 'UpliftStartedMetric');
