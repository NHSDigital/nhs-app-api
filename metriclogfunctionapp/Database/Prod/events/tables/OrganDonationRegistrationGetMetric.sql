DO $$
BEGIN

    CREATE TABLE IF NOT EXISTS events."OrganDonationRegistrationGetMetric" (
        "Timestamp" timestamp with time zone NOT NULL,
        "SessionId" character varying(36) NULL
    );

    ALTER TABLE events."OrganDonationRegistrationGetMetric" ADD COLUMN IF NOT EXISTS "AuditId" character varying (36) NULL;

    CALL perms.apply_etl_table_permissions('events', 'OrganDonationRegistrationGetMetric');

END
$$;