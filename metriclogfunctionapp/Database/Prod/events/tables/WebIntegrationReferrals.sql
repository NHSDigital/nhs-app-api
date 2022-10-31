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
END$$;