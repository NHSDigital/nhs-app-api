DO $$
BEGIN
    CREATE TABLE IF NOT EXISTS events."LoginMetric" (
        "Timestamp" timestamp with time zone NOT NULL,
        "OdsCode" character varying(6) NOT NULL,
        "LoginId" character varying NOT NULL,
        "ProofLevel" character varying(2) NOT NULL,
        "LoginEventId" character varying(36) NULL,
        "Referrer" text NULL,
        "SessionId" character varying(36) NULL,
        "AuditId" character varying(36) NULL
    );

    CALL perms.apply_etl_table_permissions('events', 'LoginMetric');

    CREATE INDEX IF NOT EXISTS LoginMetic_ProofLevel_Timestamp_LoginId_idx on events."LoginMetric" ("ProofLevel", "Timestamp", "LoginId");
    CREATE INDEX IF NOT EXISTS LoginMetic_Timestamp_idx on events."LoginMetric" ("Timestamp");
    CREATE INDEX IF NOT EXISTS LoginMetric_Timestamp_OdsCode_LoginId_ProofLevel_idx on events."LoginMetric" ("Timestamp", "OdsCode", "LoginId", "ProofLevel");
    CREATE INDEX IF NOT EXISTS loginmetric_sessionid_idx on events."LoginMetric" ("SessionId");

    IF NOT EXISTS (
        SELECT con.*
        FROM pg_catalog.pg_constraint con
            INNER JOIN pg_catalog.pg_class rel ON rel.oid = con.conrelid
            INNER JOIN pg_catalog.pg_namespace nsp ON nsp.oid = connamespace
        WHERE nsp.nspname = 'events'
            AND rel.relname = 'LoginMetric'
            AND con.conname = 'loginmetric_auditid_unique'
    ) 
    THEN
        ALTER TABLE events."LoginMetric"
            ADD CONSTRAINT loginmetric_auditid_unique UNIQUE ("AuditId");
    END IF;
END$$;