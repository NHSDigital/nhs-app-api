
CREATE TABLE IF NOT EXISTS events."AppleDownloadsSnapshot" (
    "Timestamp" timestamp NOT NULL,
    "Version" character varying NOT NULL,
    "Units" int NOT NULL
);

CALL perms.apply_etl_table_permissions('events', 'AppleDownloadsSnapshot');