CREATE INDEX IF NOT EXISTS OrganDonationRegistrationCreateMetric_SessionId_idx 
ON events."OrganDonationRegistrationCreateMetric" ("SessionId");

CREATE INDEX IF NOT EXISTS OrganDonationRegistrationCreateMetric_Timestamp_idx 
ON events."OrganDonationRegistrationCreateMetric" ("Timestamp");

CREATE INDEX IF NOT EXISTS OrganDonationRegistrationCreateMetric_Timestamp_SessionId_idx 
ON events."OrganDonationRegistrationCreateMetric" ("Timestamp", "SessionId");
