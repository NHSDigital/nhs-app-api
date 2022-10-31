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

    CALL perms.apply_etl_table_permissions('events', 'SilverIntegrationJumpOffMetric');


END
$$;