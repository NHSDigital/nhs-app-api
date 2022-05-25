
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

CREATE INDEX IF NOT EXISTS MedicalRecordViewMetric_SessionId_idx on events."MedicalRecordViewMetric" ("SessionId");
CREATE INDEX IF NOT EXISTS MedicalRecordViewMetric_Timestamp_SessionId_idx on events."MedicalRecordViewMetric" ("Timestamp", "SessionId");
CREATE INDEX IF NOT EXISTS MedicalRecordViewMetric_date_idx on events."MedicalRecordViewMetric" ("Timestamp");
CREATE INDEX IF NOT EXISTS medicalrecordviewmetric_timestamp_hassummaryrecordaccess_idx ON events."MedicalRecordViewMetric" ("Timestamp") WHERE "HasSummaryRecordAccess";
CREATE INDEX IF NOT EXISTS medicalrecordviewmetric_timestamp_hasdetailedrecordaccess_idx ON events."MedicalRecordViewMetric" ("Timestamp") WHERE "HasDetailedRecordAccess";
