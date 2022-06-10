CREATE INDEX IF NOT EXISTS OrganDonationRegistrationWithdrawMetric_SessionId_idx 
ON events."OrganDonationRegistrationWithdrawMetric" ("SessionId");

CREATE INDEX IF NOT EXISTS OrganDonationRegistrationWithdrawMetric_Timestamp_idx 
ON events."OrganDonationRegistrationWithdrawMetric" ("Timestamp");

CREATE INDEX IF NOT EXISTS OrganDonationRegistrationWithdrawMetric_Timestamp_SessionId_idx 
ON events."OrganDonationRegistrationWithdrawMetric" ("Timestamp", "SessionId");
