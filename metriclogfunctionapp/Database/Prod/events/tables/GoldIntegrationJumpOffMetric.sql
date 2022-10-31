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

CALL perms.apply_etl_table_permissions('events', 'GoldIntegrationJumpOffMetric');

END
$$;
