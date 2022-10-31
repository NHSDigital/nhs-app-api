CREATE INDEX IF NOT EXISTS ComsHubMessagesSent_LoginId_Timestamp_idx on events."CommsHubMessagesSent" ("LoginId", "Timestamp");
CREATE INDEX IF NOT EXISTS ComsHubMessagesSent_RequestId_idx on events."CommsHubMessagesSent" ("RequestId");
