DO $$
BEGIN
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

    CREATE INDEX IF NOT EXISTS OrganDonationRegistrationCreateMetric_SessionId_idx
        ON events."OrganDonationRegistrationCreateMetric" ("SessionId");

    CREATE INDEX IF NOT EXISTS OrganDonationRegistrationCreateMetric_Timestamp_idx
        ON events."OrganDonationRegistrationCreateMetric" ("Timestamp");

    CREATE INDEX IF NOT EXISTS OrganDonationRegistrationCreateMetric_Timestamp_SessionId_idx
        ON events."OrganDonationRegistrationCreateMetric" ("Timestamp", "SessionId");
END
$$;