CREATE INDEX IF NOT EXISTS MedicalRecordViewMetric_SessionId_idx on events."MedicalRecordViewMetric" ("SessionId");
CREATE INDEX IF NOT EXISTS MedicalRecordViewMetric_Timestamp_SessionId_idx on events."MedicalRecordViewMetric" ("Timestamp", "SessionId");
CREATE INDEX IF NOT EXISTS MedicalRecordViewMetric_date_idx on events."MedicalRecordViewMetric" ("Timestamp");
CREATE INDEX IF NOT EXISTS medicalrecordviewmetric_timestamp_hassummaryrecordaccess_idx ON events."MedicalRecordViewMetric" ("Timestamp") WHERE "HasSummaryRecordAccess";
CREATE INDEX IF NOT EXISTS medicalrecordviewmetric_timestamp_hasdetailedrecordaccess_idx ON events."MedicalRecordViewMetric" ("Timestamp") WHERE "HasDetailedRecordAccess";
