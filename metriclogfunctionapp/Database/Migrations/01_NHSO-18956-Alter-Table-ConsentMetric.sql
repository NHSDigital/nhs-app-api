ALTER TABLE events."ConsentMetric"
    ADD COLUMN IF NOT EXISTS "SessionId" character varying(36) NULL,
    ADD COLUMN IF NOT EXISTS "AuditId" character varying(36) NULL,
    ALTER COLUMN "OdsCode" DROP NOT NULL;

ALTER TABLE events."ConsentMetric"
    DROP CONSTRAINT IF EXISTS consentmetric_auditid_unique,
    ADD CONSTRAINT consentmetric_auditid_unique UNIQUE ("AuditId");