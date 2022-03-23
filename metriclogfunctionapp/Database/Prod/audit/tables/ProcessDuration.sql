CREATE SCHEMA IF NOT EXISTS audit;

CREATE TABLE IF NOT EXISTS audit."ProcessDuration"
(
    "Id" SERIAL PRIMARY KEY,
    "ProcessName" varchar COLLATE "default",
    "StartDateTime" timestamp with time zone,
    "EndDateTime" timestamp with time zone,
    "Duration" int
);

CALL perms.apply_compute_table_permissions('audit', 'ProcessDuration');