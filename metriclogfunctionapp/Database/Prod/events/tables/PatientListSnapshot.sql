
CREATE TABLE IF NOT EXISTS events."PatientListSnapshot" (
    "Timestamp" timestamp with time zone NOT NULL,
    "OdsCode" character varying NOT NULL,
    "PatientSize" INT NULL
);

CALL perms.apply_etl_table_permissions('events', 'PatientListSnapshot');
