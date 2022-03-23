
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

CALL perms.apply_etl_table_permissions('events', 'Device');

CREATE INDEX IF NOT EXISTS Device_SessionId_idx on events."Device" ("SessionId");
CREATE INDEX IF NOT EXISTS Device_Timestamp_idx on events."Device" ("Timestamp");
CREATE INDEX IF NOT EXISTS Device_UserAgent_idx on events."Device" ("UserAgent");
CREATE INDEX IF NOT EXISTS Device_UserAgent_idx1 on events."Device" ("UserAgent");
CREATE INDEX IF NOT EXISTS Device_date_idx on events."Device" ("Timestamp");
