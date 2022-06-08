DO $$
BEGIN

    CREATE TABLE IF NOT EXISTS events."InitialPromptMetric" (
        "LoginId" character varying(36) NULL,
        "Timestamp" timestamp with time zone NOT NULL,
        "OptedIn" character varying(3) NULL,
        "AuditId" character varying(36) NULL
    );

    CALL perms.apply_etl_table_permissions('events', 'InitialPromptMetric');

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

END$$;