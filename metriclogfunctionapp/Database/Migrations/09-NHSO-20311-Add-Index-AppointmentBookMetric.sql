CREATE INDEX IF NOT EXISTS AppointmentBookMetric_SessionId_idx
ON events."AppointmentBookMetric" ("SessionId");

CREATE INDEX IF NOT EXISTS AppointmentBookMetric_Timestamp_SessionId_idx
ON events."AppointmentBookMetric" ("Timestamp", "SessionId");
