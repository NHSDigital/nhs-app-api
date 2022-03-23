ALTER TABLE events."LoginMetric"
    ADD COLUMN IF NOT EXISTS "AuditId" character varying(36) NULL,
    ALTER COLUMN "LoginEventId" DROP NOT NULL;

ALTER TABLE events."LoginMetric"
    DROP CONSTRAINT IF EXISTS loginmetric_auditid_unique,
    ADD CONSTRAINT loginmetric_auditid_unique UNIQUE ("AuditId");    