DO $$
BEGIN

    CREATE TABLE IF NOT EXISTS events."LastLoginPatientIdentifier" (
        "LoginId" character varying(36) NULL,
        "NhsNumber" character varying (36) NULL,
        "Timestamp" timestamp with time zone NOT NULL,
        "AuditId" character varying (36) NULL,
        CONSTRAINT lastloginpatientidentifier_loginidnhsnumber_pk PRIMARY KEY ("LoginId", "NhsNumber")
    );
    CALL perms.apply_legacy_insert_update_table_permissions('events', 'LastLoginPatientIdentifier');

END$$;