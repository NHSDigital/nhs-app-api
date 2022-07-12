CREATE INDEX IF NOT EXISTS InitialPromptMetric_Timestamp_idx 
ON events."InitialPromptMetric" ("Timestamp");

CREATE INDEX IF NOT EXISTS InitialPromptMetric_LoginId_idx 
ON events."InitialPromptMetric" ("LoginId");

CREATE INDEX IF NOT EXISTS InitialPromptMetric_LoginId_Timestamp_idx 
ON events."InitialPromptMetric" ("LoginId", "Timestamp");
