ALTER TABLE events."SecondaryCareSummaryMetric"
    DROP CONSTRAINT IF EXISTS secondarycaresummarymetric_auditid_unique,
    ADD CONSTRAINT secondarycaresummarymetric_auditid_unique UNIQUE ("AuditId");