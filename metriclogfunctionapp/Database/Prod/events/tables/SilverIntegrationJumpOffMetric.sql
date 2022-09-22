DO $$
BEGIN

    CREATE TABLE IF NOT EXISTS events."SilverIntegrationJumpOffMetric" (
        "Timestamp" timestamp with time zone NOT NULL,
        "SessionId" character varying NOT NULL,
        "ProviderId" text NOT NULL,
        "ProviderName" text NOT NULL,
        "JumpOffId" text NOT NULL
    );

    ALTER TABLE events."SilverIntegrationJumpOffMetric"
        ADD COLUMN IF NOT EXISTS "AuditId" character varying (36) NULL;

    IF NOT EXISTS (
        SELECT con.*
        FROM pg_catalog.pg_constraint con
            INNER JOIN pg_catalog.pg_class rel ON rel.oid = con.conrelid
            INNER JOIN pg_catalog.pg_namespace nsp ON nsp.oid = connamespace
        WHERE nsp.nspname = 'events'
            AND rel.relname = 'SilverIntegrationJumpOffMetric'
            AND con.conname = 'silverintegrationjumpoffmetric_auditid_unique'
    )
    THEN
        ALTER TABLE events."SilverIntegrationJumpOffMetric"
            ADD CONSTRAINT silverintegrationjumpoffmetric_auditid_unique UNIQUE ("Timestamp", "SessionId", "AuditId");
    END IF;

    CALL perms.apply_etl_table_permissions('events', 'SilverIntegrationJumpOffMetric');

    CREATE INDEX IF NOT EXISTS silverjumpoffmetric_timestamp_idx on events."SilverIntegrationJumpOffMetric" ("Timestamp");
    CREATE INDEX IF NOT EXISTS silverjumpoffmetric_sessionid_idx on events."SilverIntegrationJumpOffMetric" ("SessionId");
    CREATE INDEX IF NOT EXISTS silverjumpoffmetric_timestamp_sessionid_idx on events."SilverIntegrationJumpOffMetric" ("Timestamp", "SessionId");


END
$$;