DO $$
BEGIN
    CREATE TABLE IF NOT EXISTS events."ConsentMetric" (
        "Timestamp" timestamp with time zone NOT NULL,
        "OdsCode" character varying(6) NULL,
        "LoginId" character varying(36) NOT NULL,
        "ProofLevel" character varying(2) NOT NULL,
        "SessionId" character varying(36) NULL,
        "AuditId" character varying(36) NULL
    );

    CALL perms.apply_etl_table_permissions('events', 'ConsentMetric');

    CREATE INDEX IF NOT EXISTS ConsentMetric_LoginId_ProofLevel_idx on events."ConsentMetric" ("LoginId", "ProofLevel");
    CREATE INDEX IF NOT EXISTS LoginMetric_Timestamp_OdsCode_LoginId_ProofLevel_idx on events."ConsentMetric" ("Timestamp", "OdsCode", "LoginId", "ProofLevel");
    CREATE INDEX IF NOT EXISTS ConsentMetric_date_idx on events."ConsentMetric" ("Timestamp");
    CREATE INDEX IF NOT EXISTS consentmetric_loginid_prooflevel_idx on events."ConsentMetric" ("LoginId", "ProofLevel");

    IF NOT EXISTS (
        SELECT con.*
        FROM pg_catalog.pg_constraint con
            INNER JOIN pg_catalog.pg_class rel ON rel.oid = con.conrelid
            INNER JOIN pg_catalog.pg_namespace nsp ON nsp.oid = connamespace
        WHERE nsp.nspname = 'events'
            AND rel.relname = 'ConsentMetric'
            AND con.conname = 'consentmetric_auditid_unique'
    ) 
    THEN
        ALTER TABLE events."ConsentMetric"
            ADD CONSTRAINT consentmetric_auditid_unique UNIQUE ("AuditId");
    END IF;
END$$;