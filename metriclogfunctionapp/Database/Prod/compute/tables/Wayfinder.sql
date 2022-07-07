CREATE TABLE IF NOT EXISTS compute."Wayfinder" (
    "Date" date NOT NULL,
    "TotalSessions" bigint,
    "TotalViews" bigint,
    "Users" bigint,
    "TotalReferrals" bigint,
    "TotalUpcomingAppointments" bigint,
    CONSTRAINT "Wayfinder_pkey" PRIMARY KEY ("Date")
);

CALL perms.apply_compute_table_permissions('compute', 'Wayfinder');