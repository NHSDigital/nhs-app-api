DO $$
BEGIN

    IF NOT EXISTS (
        SELECT con.*
        FROM pg_catalog.pg_constraint con
            INNER JOIN pg_catalog.pg_class rel ON rel.oid = con.conrelid
            INNER JOIN pg_catalog.pg_namespace nsp ON nsp.oid = connamespace
        WHERE nsp.nspname = 'events'
            AND rel.relname = 'OrganDonationRegistrationUpdateMetric'
            AND con.conname = 'organdonationregistrationupdatemetric_auditid_unique'
    )
    THEN
        ALTER TABLE events."OrganDonationRegistrationUpdateMetric"
            ADD CONSTRAINT organdonationregistrationupdatemetric_auditid_unique UNIQUE ("AuditId");
    END IF;

    CREATE INDEX IF NOT EXISTS OrganDonationRegistrationUpdateMetric_SessionId_idx
        ON events."OrganDonationRegistrationUpdateMetric" ("SessionId");

    CREATE INDEX IF NOT EXISTS OrganDonationRegistrationUpdateMetric_Timestamp_idx
        ON events."OrganDonationRegistrationUpdateMetric" ("Timestamp");

    CREATE INDEX IF NOT EXISTS OrganDonationRegistrationUpdateMetric_Timestamp_SessionId_idx
        ON events."OrganDonationRegistrationUpdateMetric" ("Timestamp", "SessionId");

END
$$;