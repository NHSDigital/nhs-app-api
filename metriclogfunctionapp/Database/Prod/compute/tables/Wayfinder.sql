CREATE TABLE IF NOT EXISTS compute."Wayfinder" (
    "Date" date NOT NULL,
    "Sessions" bigint,
    "Views" bigint,
    "TotalReferrals" bigint,
    "TotalAppts" bigint,
    "SessionsWithReferrals" bigint,
    "SessionsWithAppts" bigint,
    "ViewsWithReferrals" bigint,
    "ViewsWithAppts" bigint,
    "Neither" bigint,
    "Both" bigint,
    "Either" bigint,
    "MostAppts" bigint,
    "MostReferrals" bigint,
    CONSTRAINT "Wayfinder_pkey" PRIMARY KEY ("Date")
);

CALL perms.apply_compute_table_permissions('compute', 'Wayfinder');