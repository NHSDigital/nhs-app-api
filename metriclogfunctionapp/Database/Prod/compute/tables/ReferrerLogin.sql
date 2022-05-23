CREATE TABLE IF NOT EXISTS compute."ReferrerLogin" (
    "Date" date,
    "ReferrerId" text NOT NULL,
    "ExistingUsers" Integer DEFAULT 0,
    "NewUsers" Integer DEFAULT 0,
    CONSTRAINT referrerlogin_date_referrerId_unique UNIQUE ("Date", "ReferrerId")
);
CALL perms.apply_compute_table_permissions('compute', 'ReferrerLogin');