CREATE INDEX IF NOT EXISTS LoginPatientIdentifier_LoginEventId_idx on events."LoginPatientIdentifier" ("LoginEventId");
CREATE INDEX IF NOT EXISTS LoginPatientIdentifier_date_idx on events."LoginPatientIdentifier" ("Timestamp");
