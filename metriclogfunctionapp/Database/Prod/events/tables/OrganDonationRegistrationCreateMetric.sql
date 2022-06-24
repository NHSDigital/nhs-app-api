DO $$
BEGIN
    CREATE TABLE IF NOT EXISTS events."OrganDonationRegistrationCreateMetric" (
        "Timestamp" timestamp with time zone NOT NULL,
        "SessionId" character varying(36) NOT NULL
    );

    ALTER TABLE events."OrganDonationRegistrationCreateMetric"
        ADD COLUMN IF NOT EXISTS "AuditId" character varying (36) NULL;

    IF NOT EXISTS (
        SELECT con.*
        FROM pg_catalog.pg_constraint con
            INNER JOIN pg_catalog.pg_class rel ON rel.oid = con.conrelid
            INNER JOIN pg_catalog.pg_namespace nsp ON nsp.oid = connamespace
        WHERE nsp.nspname = 'events'
            AND rel.relname = 'OrganDonationRegistrationCreateMetric'
            AND con.conname = 'organdonationregistrationcreatemetric_auditid_unique'
    )
    THEN
        ALTER TABLE events."OrganDonationRegistrationCreateMetric"
            ADD CONSTRAINT organdonationregistrationcreatemetric_auditid_unique UNIQUE ("AuditId");
    END IF;

    CALL perms.apply_etl_table_permissions('events', 'OrganDonationRegistrationCreateMetric');
END
$$;