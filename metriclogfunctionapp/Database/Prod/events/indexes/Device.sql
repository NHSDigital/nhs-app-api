DO $$
BEGIN

    IF NOT EXISTS (
        SELECT con.*
        FROM pg_catalog.pg_constraint con
            INNER JOIN pg_catalog.pg_class rel ON rel.oid = con.conrelid
            INNER JOIN pg_catalog.pg_namespace nsp ON nsp.oid = connamespace
        WHERE nsp.nspname = 'events'
            AND rel.relname = 'Device'
            AND con.conname = 'device_auditid_unique'
    )
    THEN
        ALTER TABLE events."Device"
            ADD CONSTRAINT device_auditid_unique UNIQUE ("Timestamp", "SessionId", "AuditId");
    END IF;

    CREATE INDEX IF NOT EXISTS Device_SessionId_idx on events."Device" ("SessionId");
    CREATE INDEX IF NOT EXISTS Device_Timestamp_idx on events."Device" ("Timestamp");
    CREATE INDEX IF NOT EXISTS Device_UserAgent_idx on events."Device" ("UserAgent");
    CREATE INDEX IF NOT EXISTS device_timestamp_session_idx on events."Device" ("Timestamp", "SessionId");
    DROP INDEX IF EXISTS events."device_useragent_idx1";
	DROP INDEX IF EXISTS events."device_date_idx";

END$$;
