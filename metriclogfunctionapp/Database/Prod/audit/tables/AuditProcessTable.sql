CREATE SCHEMA IF NOT EXISTS audit;

CREATE TABLE IF NOT EXISTS audit."Process" (
    "Id" SERIAL PRIMARY KEY,
    "ProcessName" TEXT,
    "RangeStartDateTime" timestamp with time zone NOT NULL,
    "RangeEndDateTime" timestamp with time zone,
    "ProcessStartDateTime" timestamp with time zone NOT NULL,
    "ProcessEndDateTime" timestamp with time zone NOT NULL,
    "Duration" interval,
    "IsSuccess" BOOLEAN
);

CALL perms.apply_compute_table_permissions('audit', 'Process');
CALL perms.apply_sequence_permissions('audit', 'Process', 'Id');