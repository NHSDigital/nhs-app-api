DO $$
BEGIN

    CREATE INDEX IF NOT EXISTS LastLoginPatientIdentifier_LastLoginTimestamp_idx
        on events."LastLoginPatientIdentifier" ("Timestamp");

    CREATE INDEX IF NOT EXISTS LastLoginPatientIdentifier_Loginid_idx
        on events."LastLoginPatientIdentifier" ("LoginId");

    CREATE INDEX IF NOT EXISTS LastLoginPatientIdentifier_Timestamp_LoginId_idx
        on events."LastLoginPatientIdentifier" ("Timestamp", "LoginId");

    CREATE INDEX IF NOT EXISTS LastLoginPatientIdentifier_NhsNumber_idx
        on events."LastLoginPatientIdentifier" ("NhsNumber");

    IF NOT EXISTS (
        SELECT con.*
        FROM pg_catalog.pg_constraint con
            INNER JOIN pg_catalog.pg_class rel ON rel.oid = con.conrelid
            INNER JOIN pg_catalog.pg_namespace nsp ON nsp.oid = connamespace
        WHERE nsp.nspname = 'events'
            AND rel.relname = 'LastLoginPatientIdentifier'
            AND con.conname = 'lastloginpatientidentifier_auditid_unique'
    )
    THEN
        ALTER TABLE events."LastLoginPatientIdentifier"
            ADD CONSTRAINT lastloginpatientidentifier_auditid_unique UNIQUE ("AuditId");
    END IF;

END$$;