CREATE INDEX IF NOT EXISTS UpliftStartedMetric_SessionId_idx on events."UpliftStartedMetric" ("SessionId");
CREATE INDEX IF NOT EXISTS UpliftStartedMetric_date_idx on events."UpliftStartedMetric" ("Timestamp");
