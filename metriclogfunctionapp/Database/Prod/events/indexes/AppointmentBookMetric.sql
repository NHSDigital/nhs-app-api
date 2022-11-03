DO $$
BEGIN
    CREATE INDEX IF NOT EXISTS appointmentbookmetric_date_idx on events."AppointmentBookMetric" ("Timestamp");

    CREATE INDEX IF NOT EXISTS AppointmentBookMetric_SessionId_idx
        ON events."AppointmentBookMetric" ("SessionId");

    CREATE INDEX IF NOT EXISTS AppointmentBookMetric_Timestamp_SessionId_idx
        ON events."AppointmentBookMetric" ("Timestamp", "SessionId");

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