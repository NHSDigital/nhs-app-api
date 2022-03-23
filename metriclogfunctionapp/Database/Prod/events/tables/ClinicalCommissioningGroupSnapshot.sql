
CREATE TABLE IF NOT EXISTS events."ClinicalCommissioningGroupSnapshot" (
    "Timestamp" timestamp with time zone NOT NULL,
    "CCGCode" character varying NOT NULL,
    "Name" text,
    "Status" text,
    "PostCode" text
);

CALL perms.apply_etl_table_permissions('events', 'ClinicalCommissioningGroupSnapshot');
