CREATE TABLE IF NOT EXISTS events."SecondaryCareSummaryMetric" (
    "Timestamp" timestamp with time zone NOT NULL,
    "SessionId" character varying(36) NULL,
    "TotalReferrals" int,
    "TotalUpcomingAppointments" int,
    "AuditId" character varying (36) NULL
);

CALL perms.apply_etl_table_permissions('events', 'SecondaryCareSummaryMetric');