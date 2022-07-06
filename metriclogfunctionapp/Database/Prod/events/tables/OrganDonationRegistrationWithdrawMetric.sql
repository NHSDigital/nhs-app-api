DO $$
BEGIN

    CREATE TABLE IF NOT EXISTS events."OrganDonationRegistrationWithdrawMetric" (
        "Timestamp" timestamp with time zone NOT NULL,
        "SessionId" character varying(36) NOT NULL
    );

    ALTER TABLE events."OrganDonationRegistrationWithdrawMetric"
        ADD COLUMN IF NOT EXISTS "AuditId" character varying (36) NULL;

    CREATE INDEX IF NOT EXISTS organdonationregistrationwithdrawmetric_date_idx
        on events."OrganDonationRegistrationWithdrawMetric" ("Timestamp");

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

    CALL perms.apply_etl_table_permissions('events', 'OrganDonationRegistrationWithdrawMetric');
    
END 
$$;