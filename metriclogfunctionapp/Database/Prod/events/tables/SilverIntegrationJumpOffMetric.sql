
CREATE TABLE IF NOT EXISTS events."SilverIntegrationJumpOffMetric" (
    "Timestamp" timestamp with time zone NOT NULL,
    "SessionId" character varying NOT NULL,
    "ProviderId" text NOT NULL,
    "ProviderName" text NOT NULL,
    "JumpOffId" text NOT NULL
);

CALL perms.apply_etl_table_permissions('events', 'SilverIntegrationJumpOffMetric');

CREATE INDEX IF NOT EXISTS silverjumpoffmetric_timestamp_idx on events."SilverIntegrationJumpOffMetric" ("Timestamp");
