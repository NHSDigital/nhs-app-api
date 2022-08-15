DO $$
BEGIN

CREATE TABLE IF NOT EXISTS events."GoldIntegrationJumpOffMetric" (
    "Timestamp" timestamp with time zone NOT NULL,
    "SessionId" character varying NOT NULL,
    "ProviderId" character varying NOT NULL,
    "ProviderName" character varying NOT NULL,
    "JumpOffId" character varying NOT NULL,
    "AuditId" character varying NOT NULL
);

IF NOT EXISTS (
        SELECT con.*
        FROM pg_catalog.pg_constraint con
            INNER JOIN pg_catalog.pg_class rel ON rel.oid = con.conrelid
            INNER JOIN pg_catalog.pg_namespace nsp ON nsp.oid = connamespace
        WHERE nsp.nspname = 'events'
            AND rel.relname = 'GoldIntegrationJumpOffMetric'
            AND con.conname = 'goldintegrationjumpoffmetric_auditid_unique'
    )
    THEN
        ALTER TABLE events."GoldIntegrationJumpOffMetric"
            ADD CONSTRAINT goldintegrationjumpoffmetric_auditid_unique UNIQUE ("AuditId");
    END IF;

CALL perms.apply_etl_table_permissions('events', 'GoldIntegrationJumpOffMetric');

CREATE INDEX IF NOT EXISTS goldjumpoffmetric_timestamp_idx on events."GoldIntegrationJumpOffMetric" ("Timestamp");
CREATE INDEX IF NOT EXISTS goldjumpoffmetric_sessionid_idx on events."GoldIntegrationJumpOffMetric" ("SessionId");
CREATE INDEX IF NOT EXISTS goldjumpoffmetric_timestamp_sessionid_idx on events."GoldIntegrationJumpOffMetric" ("Timestamp", "SessionId");

END
$$;
