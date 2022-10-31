
CREATE TABLE IF NOT EXISTS events."LoginPatientIdentifier" (
    "Timestamp" timestamp with time zone NOT NULL,
    "LoginEventId" character varying NOT NULL,
    "PatientIdentifier" character varying NOT NULL
);

CALL perms.apply_etl_table_permissions('events', 'LoginPatientIdentifier');
