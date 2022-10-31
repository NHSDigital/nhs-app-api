CREATE INDEX IF NOT EXISTS ComsHubMessagesRead_Timestamp_MessageId_ReadTimestamp_idx on events."CommsHubMessagesRead" ("Timestamp", "MessageId", "ReadTimestamp");
