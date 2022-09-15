DO $$
BEGIN
    CREATE TABLE IF NOT EXISTS events."WebIntegrationReferrals" (
                                                        "Timestamp" timestamp with time zone NOT NULL,
                                                        "Referrer" text NOT NULL,
                                                        "SessionId" character varying(36) NULL,
                                                        "AuditId" character varying(36) NULL
    );

    ALTER TABLE events."WebIntegrationReferrals"
        ADD COLUMN IF NOT EXISTS "ReferrerOrigin" text NULL;

    CALL perms.apply_etl_table_permissions('events', 'WebIntegrationReferrals');

    IF NOT EXISTS (
        SELECT con.*
        FROM pg_catalog.pg_constraint con
            INNER JOIN pg_catalog.pg_class rel ON rel.oid = con.conrelid
            INNER JOIN pg_catalog.pg_namespace nsp ON nsp.oid = connamespace
        WHERE nsp.nspname = 'events'
            AND rel.relname = 'WebIntegrationReferrals'
            AND con.conname = 'webintegrationrefferrals_auditid_unique'
    )
    THEN
        ALTER TABLE events."WebIntegrationReferrals"
            ADD CONSTRAINT webintegrationrefferrals_auditid_unique UNIQUE ("AuditId");
    END IF;
END$$;