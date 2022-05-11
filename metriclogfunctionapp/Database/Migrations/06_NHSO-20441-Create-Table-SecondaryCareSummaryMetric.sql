CREATE TABLE IF NOT EXISTS events."SecondaryCareSummaryMetric" (
    "Timestamp" timestamp with time zone NOT NULL,
    "SessionId" character varying(36) NULL,
    "TotalReferrals" int,
    "TotalUpcomingAppointments" int,
    "AuditId" character varying (36) NULL
);

ALTER TABLE events."SecondaryCareSummaryMetric"
    DROP CONSTRAINT IF EXISTS secondarycaresummarymetric_auditid_unique,
    ADD CONSTRAINT secondarycaresummarymetric_auditid_unique UNIQUE ("AuditId");

CALL perms.apply_etl_table_permissions('events', 'SecondaryCareSummaryMetric');