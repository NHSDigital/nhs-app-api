CREATE INDEX IF NOT EXISTS UpliftMetric_ProofLevel_idx on events."UpliftMetric" ("ProofLevel");
CREATE UNIQUE INDEX IF NOT EXISTS UpliftMetric_Timestamp_OdsCode_loginId_ProofLevel_Action_idx on events."UpliftMetric" ("Timestamp", "OdsCode", "LoginId", "ProofLevel", "Action");
CREATE INDEX IF NOT EXISTS UpliftMetric_Timestamp_OdsCode_loginId_ProofLevel_idx on events."UpliftMetric" ("Timestamp", "OdsCode", "LoginId", "ProofLevel");
CREATE INDEX IF NOT EXISTS UpliftMetric_date_idx on events."UpliftMetric" ("Timestamp");
