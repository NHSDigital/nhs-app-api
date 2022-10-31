ALTER TABLE events."AppointmentCancelMetric"
    DROP CONSTRAINT IF EXISTS appointmentcancelmetric_auditid_unique,
    ADD CONSTRAINT appointmentcancelmetric_auditid_unique UNIQUE ("AuditId");

CREATE INDEX IF NOT EXISTS AppointmentCancelMetric_date_idx on events."AppointmentCancelMetric" ("Timestamp");
