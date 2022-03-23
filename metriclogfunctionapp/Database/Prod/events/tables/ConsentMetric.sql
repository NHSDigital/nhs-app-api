
CREATE TABLE IF NOT EXISTS events."ConsentMetric" (
    "Timestamp" timestamp with time zone NOT NULL,
    "OdsCode" character varying(6) NOT NULL,
    "LoginId" character varying(36) NOT NULL,
    "ProofLevel" character varying(2) NOT NULL
);

CALL perms.apply_etl_table_permissions('events', 'ConsentMetric');

CREATE INDEX IF NOT EXISTS ConsentMetric_LoginId_ProofLevel_idx on events."ConsentMetric" ("LoginId", "ProofLevel");
CREATE INDEX IF NOT EXISTS LoginMetric_Timestamp_OdsCode_LoginId_ProofLevel_idx on events."ConsentMetric" ("Timestamp", "OdsCode", "LoginId", "ProofLevel");
CREATE INDEX IF NOT EXISTS ConsentMetric_date_idx on events."ConsentMetric" ("Timestamp");
CREATE INDEX IF NOT EXISTS consentmetric_loginid_prooflevel_idx on events."ConsentMetric" ("LoginId", "ProofLevel");
