DO $$
BEGIN

    IF NOT EXISTS (
        SELECT con.*
        FROM pg_catalog.pg_constraint con
            INNER JOIN pg_catalog.pg_class rel ON rel.oid = con.conrelid
            INNER JOIN pg_catalog.pg_namespace nsp ON nsp.oid = connamespace
        WHERE nsp.nspname = 'events'
            AND rel.relname = 'InitialPromptMetric'
            AND con.conname = 'initialpromptmetric_auditid_unique'
    )
    THEN
        ALTER TABLE events."InitialPromptMetric"
            ADD CONSTRAINT initialpromptmetric_auditid_unique UNIQUE ("AuditId");
    END IF;

    CREATE INDEX IF NOT EXISTS InitialPromptMetric_Timestamp_idx
        ON events."InitialPromptMetric" ("Timestamp");

    CREATE INDEX IF NOT EXISTS InitialPromptMetric_LoginId_idx
        ON events."InitialPromptMetric" ("LoginId");

    CREATE INDEX IF NOT EXISTS InitialPromptMetric_LoginId_Timestamp_idx
        ON events."InitialPromptMetric" ("LoginId", "Timestamp");

END$$;