DO $$
BEGIN

    CREATE TABLE IF NOT EXISTS events."Device" (
        "Timestamp" timestamp with time zone NOT NULL,
        "SessionId" character varying NOT NULL,
        "AppVersion" character varying NULL,
        "DeviceManufacturer" character varying NULL,
        "DeviceModel" character varying NULL,
        "DeviceOS" character varying NULL,
        "DeviceOSVersion" character varying NULL,
        "UserAgent" text NOT NULL
    );

    ALTER TABLE events."Device"
        ADD COLUMN IF NOT EXISTS "AuditId" character varying (36) NULL;

    CALL perms.apply_etl_table_permissions('events', 'Device');

END$$;
