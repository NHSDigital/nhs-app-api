DO $$
BEGIN
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

    CREATE INDEX IF NOT EXISTS WebIntegrationReferrals_SessionId_idx
        ON events."WebIntegrationReferrals" ("SessionId");

    CREATE INDEX IF NOT EXISTS WebIntegrationReferrals_Timestamp_idx
        ON events."WebIntegrationReferrals" ("Timestamp");

    CREATE INDEX IF NOT EXISTS WebIntegrationReferrals_Referrer_idx
        ON events."WebIntegrationReferrals" ("Referrer");

    CREATE INDEX IF NOT EXISTS WebIntegrationReferrals_Timestamp_SessionId_idx
        ON events."WebIntegrationReferrals" ("Timestamp", "SessionId");

    CREATE INDEX IF NOT EXISTS WebIntegrationReferrals_Timestamp_Referrer_idx
        ON events."WebIntegrationReferrals" ("Timestamp", "Referrer");
END$$;