DO $$
BEGIN
    CREATE TABLE IF NOT EXISTS events."OrganDonationRegistrationCreateMetric" (
        "Timestamp" timestamp with time zone NOT NULL,
        "SessionId" character varying(36) NOT NULL
    );

    ALTER TABLE events."OrganDonationRegistrationCreateMetric"
        ADD COLUMN IF NOT EXISTS "AuditId" character varying (36) NULL;

    CALL perms.apply_etl_table_permissions('events', 'OrganDonationRegistrationCreateMetric');
END
$$;