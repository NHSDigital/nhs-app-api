CREATE INDEX IF NOT EXISTS WebIntegrationReferrals_SessionId_idx 
ON events."WebIntegrationReferrals" ("SessionId");

CREATE INDEX IF NOT EXISTS WebIntegrationReferrals_Timestamp_idx 
ON events."WebIntegrationReferrals" ("Timestamp");

CREATE INDEX IF NOT EXISTS WebIntegrationReferrals_Referrer_idx 
ON events."WebIntegrationReferrals" ("Referrer");

CREATE INDEX IF NOT EXISTS WebIntegrationReferrals_Timestamp_SessionId_idx 
ON events."WebIntegrationReferrals" ("Timestamp", "SessionId");

CREATE INDEX IF NOT EXISTS WebIntegrationReferrals_Timestamp_Referrer_idx 
ON events."WebIntegrationReferrals" ("Timestamp", "Referrer");
