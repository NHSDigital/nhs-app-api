CREATE INDEX IF NOT EXISTS AppointmentCancelMetric_SessionId_idx 
ON events."AppointmentCancelMetric" ("SessionId");

CREATE INDEX IF NOT EXISTS AppointmentCancelMetric_Timestamp_SessionId_idx 
ON events."AppointmentCancelMetric" ("Timestamp", "SessionId");
