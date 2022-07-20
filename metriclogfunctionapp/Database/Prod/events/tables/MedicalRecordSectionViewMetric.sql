DO $$
BEGIN

    CREATE TABLE IF NOT EXISTS events."MedicalRecordSectionViewMetric" (
        "Timestamp" timestamp with time zone NOT NULL,
        "SessionId" character varying(36) NOT NULL,
        "Supplier" character varying(36) NULL,
        "IsActingOnBehalfOfAnother" boolean,
        "Section" text NULL,
        "AuditId" character varying (36) NULL
    );

    CREATE INDEX IF NOT EXISTS medicalrecordsectionviewmetric_date_idx
        on events."MedicalRecordSectionViewMetric" ("Timestamp");

    CREATE INDEX IF NOT EXISTS medicalrecordsectionviewmetric_session_idx
        on events."MedicalRecordSectionViewMetric" ("SessionId");

    CREATE INDEX IF NOT EXISTS medicalrecordsectionviewmetric_date_session_idx
        on events."MedicalRecordSectionViewMetric" ("Timestamp", "SessionId");

    IF NOT EXISTS (
        SELECT con.*
        FROM pg_catalog.pg_constraint con
            INNER JOIN pg_catalog.pg_class rel ON rel.oid = con.conrelid
            INNER JOIN pg_catalog.pg_namespace nsp ON nsp.oid = connamespace
        WHERE nsp.nspname = 'events'
            AND rel.relname = 'MedicalRecordSectionViewMetric'
            AND con.conname = 'medicalrecordsectionviewmetric_auditid_unique'
    )
    THEN
        ALTER TABLE events."MedicalRecordSectionViewMetric"
            ADD CONSTRAINT medicalrecordsectionviewmetric_auditid_unique UNIQUE ("AuditId");
    END IF;

    CALL perms.apply_etl_table_permissions('events', 'MedicalRecordSectionViewMetric');
    
END 
$$;