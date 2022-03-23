CREATE TABLE IF NOT EXISTS events."GoogleDownloadsSnapshot" (
    "Timestamp" timestamp with time zone NOT NULL,
    "Units" int NOT NULL
);

CALL perms.apply_etl_table_permissions('events', 'GoogleDownloadsSnapshot');
