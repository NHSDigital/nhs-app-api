
CREATE TABLE IF NOT EXISTS events."GeneralPracticeSnapshot" (
    "Timestamp" timestamp with time zone NOT NULL,
    "OdsCode" character varying NOT NULL,
    "CCGCode" character varying NOT NULL,
    "Name" text,
    "Status" text,
    "PostCode" text
);

CALL perms.apply_etl_table_permissions('events', 'GeneralPracticeSnapshot');

CREATE INDEX IF NOT EXISTS GeneralPracticeSnapshot_Timestamp_OdsCode_CCGCode_Name_Stat_idx on events."GeneralPracticeSnapshot" ("Timestamp", "OdsCode", "CCGCode", "Name", "Status");
