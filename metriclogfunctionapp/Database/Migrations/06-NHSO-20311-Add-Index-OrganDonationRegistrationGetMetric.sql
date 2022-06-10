CREATE INDEX IF NOT EXISTS OrganDonationRegistrationGetMetric_SessionId_idx 
ON events."OrganDonationRegistrationGetMetric" ("SessionId");

CREATE INDEX IF NOT EXISTS OrganDonationRegistrationGetMetric_Timestamp_idx 
ON events."OrganDonationRegistrationGetMetric" ("Timestamp");

CREATE INDEX IF NOT EXISTS OrganDonationRegistrationGetMetric_Timestamp_SessionId_idx 
ON events."OrganDonationRegistrationGetMetric" ("Timestamp", "SessionId");
