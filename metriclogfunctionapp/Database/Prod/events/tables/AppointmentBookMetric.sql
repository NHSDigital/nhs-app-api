DO $$
BEGIN
    CREATE TABLE IF NOT EXISTS events."AppointmentBookMetric" (
        "Timestamp" timestamp with time zone NOT NULL,
        "SessionId" character varying NULL,
        "AuditId" character varying NULL
    );

    CALL perms.apply_etl_table_permissions('events', 'AppointmentBookMetric');

    CREATE INDEX IF NOT EXISTS appointmentbookmetric_date_idx on events."AppointmentBookMetric" ("Timestamp");

    IF NOT EXISTS (
        SELECT con.*
        FROM pg_catalog.pg_constraint con
            INNER JOIN pg_catalog.pg_class rel ON rel.oid = con.conrelid
            INNER JOIN pg_catalog.pg_namespace nsp ON nsp.oid = connamespace
        WHERE nsp.nspname = 'events'
            AND rel.relname = 'AppointmentBookMetric'
            AND con.conname = 'appointmentbookmetric_auditid_unique'
    )
    THEN
        ALTER TABLE events."AppointmentBookMetric"
            ADD CONSTRAINT appointmentbookmetric_auditid_unique UNIQUE ("AuditId");
    END IF;
END 
$$;