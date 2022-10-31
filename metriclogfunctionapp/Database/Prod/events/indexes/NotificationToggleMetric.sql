DO $$
BEGIN

    IF NOT EXISTS (
        SELECT con.*
        FROM pg_catalog.pg_constraint con
            INNER JOIN pg_catalog.pg_class rel ON rel.oid = con.conrelid
            INNER JOIN pg_catalog.pg_namespace nsp ON nsp.oid = connamespace
        WHERE nsp.nspname = 'events'
            AND rel.relname = 'NotificationToggleMetric'
            AND con.conname = 'notificationtogglemetric_auditid_unique'
    )
    THEN
        ALTER TABLE events."NotificationToggleMetric"
            ADD CONSTRAINT notificationtogglemetric_auditid_unique UNIQUE ("AuditId");
    END IF;

END$$;