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

    CALL perms.apply_etl_table_permissions('events', 'MedicalRecordSectionViewMetric');

END
$$;