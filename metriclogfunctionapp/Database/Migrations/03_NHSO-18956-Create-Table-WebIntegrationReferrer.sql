CREATE TABLE IF NOT EXISTS events."WebIntegrationReferrals" (
                                                    "Timestamp" timestamp with time zone NOT NULL,
                                                    "Referrer" text NOT NULL,
                                                    "SessionId" character varying(36) NULL,
                                                    "AuditId" character varying(36) NULL
);

ALTER TABLE events."WebIntegrationReferrals"
    DROP CONSTRAINT IF EXISTS webintegrationrefferrals_auditid_unique,
    ADD CONSTRAINT webintegrationrefferrals_auditid_unique UNIQUE ("AuditId");

CALL perms.apply_etl_table_permissions('events', 'WebIntegrationReferrals');