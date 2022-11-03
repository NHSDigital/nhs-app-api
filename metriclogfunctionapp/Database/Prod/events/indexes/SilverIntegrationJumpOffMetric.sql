DO $$
BEGIN

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

    CREATE INDEX IF NOT EXISTS SilverIntegrationJumpOffMetric_Timestamp_idx on events."SilverIntegrationJumpOffMetric" ("Timestamp");
    CREATE INDEX IF NOT EXISTS SilverIntegrationJumpOffMetric_Timestamp_SessionId_idx on events."SilverIntegrationJumpOffMetric" ("Timestamp", "SessionId");

    CREATE INDEX IF NOT EXISTS SilverIntegrationJumpOffMetric_SessionId_idx
        ON events."SilverIntegrationJumpOffMetric" ("SessionId");

    CREATE INDEX IF NOT EXISTS SilverIntegrationJumpOffMetric_ProviderName_idx
        ON events."SilverIntegrationJumpOffMetric" ("ProviderName");

    CREATE INDEX IF NOT EXISTS SilverIntegrationJumpOffMetric_SessionId_CovidJumpOff_idx
        ON events."SilverIntegrationJumpOffMetric" ("SessionId")
        WHERE "ProviderName" = 'the Department of Health and Social Care';

    CREATE INDEX IF NOT EXISTS SilverIntegrationJumpOffMetric_SessionId_OtherJumpOff_idx
        ON events."SilverIntegrationJumpOffMetric" ("SessionId")
        WHERE "ProviderName" <> 'the Department of Health and Social Care';

END
$$;