
CREATE TABLE IF NOT EXISTS events."PatientsEnabledSnapshot" (
    "Timestamp" timestamp with time zone NOT NULL,
    "OdsCode" character varying NOT NULL,
    "Supplier" text NOT NULL,
    "PatientsEnabled" INT NULL
);

CALL perms.apply_etl_table_permissions('events', 'PatientsEnabledSnapshot');
