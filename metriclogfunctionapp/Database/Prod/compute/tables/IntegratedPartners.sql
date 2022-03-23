CREATE TABLE IF NOT EXISTS compute."IntegratedPartners" (
    "Date" date,
    "OdsCode" text,
    "Provider" text,
    "JumpOff" text,
    "Clicks" int
);
CALL perms.apply_compute_table_permissions('compute', 'IntegratedPartners');