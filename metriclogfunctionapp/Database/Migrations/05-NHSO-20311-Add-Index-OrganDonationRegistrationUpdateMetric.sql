CREATE INDEX IF NOT EXISTS OrganDonationRegistrationUpdateMetric_SessionId_idx 
ON events."OrganDonationRegistrationUpdateMetric" ("SessionId");

CREATE INDEX IF NOT EXISTS OrganDonationRegistrationUpdateMetric_Timestamp_idx 
ON events."OrganDonationRegistrationUpdateMetric" ("Timestamp");

CREATE INDEX IF NOT EXISTS OrganDonationRegistrationUpdateMetric_Timestamp_SessionId_idx 
ON events."OrganDonationRegistrationUpdateMetric" ("Timestamp", "SessionId");
