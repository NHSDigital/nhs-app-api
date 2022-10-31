DO $$
BEGIN
    CREATE TABLE IF NOT EXISTS events."AppointmentBookMetric" (
        "Timestamp" timestamp with time zone NOT NULL,
        "SessionId" character varying NULL,
        "AuditId" character varying NULL
    );

    CALL perms.apply_etl_table_permissions('events', 'AppointmentBookMetric');
END
$$;