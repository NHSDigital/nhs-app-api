DO $$
BEGIN

    IF NOT EXISTS (
        SELECT con.*
        FROM pg_catalog.pg_constraint con
            INNER JOIN pg_catalog.pg_class rel ON rel.oid = con.conrelid
            INNER JOIN pg_catalog.pg_namespace nsp ON nsp.oid = connamespace
        WHERE nsp.nspname = 'events'
            AND rel.relname = 'OrganDonationRegistrationWithdrawMetric'
            AND con.conname = 'organdonationregistrationwithdrawmetric_auditid_unique'
    )
    THEN
        ALTER TABLE events."OrganDonationRegistrationWithdrawMetric"
            ADD CONSTRAINT organdonationregistrationwithdrawmetric_auditid_unique UNIQUE ("AuditId");
    END IF;

    CREATE INDEX IF NOT EXISTS OrganDonationRegistrationWithdrawMetric_SessionId_idx
        ON events."OrganDonationRegistrationWithdrawMetric" ("SessionId");

    CREATE INDEX IF NOT EXISTS OrganDonationRegistrationWithdrawMetric_Timestamp_idx
        ON events."OrganDonationRegistrationWithdrawMetric" ("Timestamp");

    CREATE INDEX IF NOT EXISTS OrganDonationRegistrationWithdrawMetric_Timestamp_SessionId_idx
        ON events."OrganDonationRegistrationWithdrawMetric" ("Timestamp", "SessionId");

END
$$;