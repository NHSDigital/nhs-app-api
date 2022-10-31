
CREATE TABLE IF NOT EXISTS events."MedicalRecordViewMetric" (
    "Timestamp" timestamp with time zone NOT NULL,
    "SessionId" character varying(36) NOT NULL,
    "HasSummaryRecordAccess" boolean,
    "HasDetailedRecordAccess" boolean
);

ALTER TABLE events."MedicalRecordViewMetric"
    ADD COLUMN IF NOT EXISTS "AuditId" character varying (36) NULL,
    DROP CONSTRAINT IF EXISTS medicalrecordviewmetric_auditid_unique,
    ADD CONSTRAINT medicalrecordviewmetric_auditid_unique UNIQUE ("AuditId");


CALL perms.apply_etl_table_permissions('events', 'MedicalRecordViewMetric');
