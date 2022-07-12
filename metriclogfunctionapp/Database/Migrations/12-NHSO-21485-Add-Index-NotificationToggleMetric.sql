CREATE INDEX IF NOT EXISTS NotificationToggleMetric_Timestamp_idx 
ON events."NotificationToggleMetric" ("Timestamp");

CREATE INDEX IF NOT EXISTS NotificationToggleMetric_LoginId_idx 
ON events."NotificationToggleMetric" ("LoginId");

CREATE INDEX IF NOT EXISTS NotificationToggleMetric_LoginId_Timestamp_idx 
ON events."NotificationToggleMetric" ("LoginId", "Timestamp");
