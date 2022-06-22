DO $$
BEGIN

    CREATE TABLE IF NOT EXISTS events."OrganDonationRegistrationGetMetric" (
        "Timestamp" timestamp with time zone NOT NULL,
        "SessionId" character varying(36) NULL
    );

    ALTER TABLE events."OrganDonationRegistrationGetMetric" ADD COLUMN IF NOT EXISTS "AuditId" character varying (36) NULL;

    CALL perms.apply_etl_table_permissions('events', 'OrganDonationRegistrationGetMetric');

    CREATE INDEX IF NOT EXISTS organdonationregistrationgetmetric_date_idx on events."OrganDonationRegistrationGetMetric" ("Timestamp");

    IF NOT EXISTS (
        SELECT con.*
        FROM pg_catalog.pg_constraint con
            INNER JOIN pg_catalog.pg_class rel ON rel.oid = con.conrelid
            INNER JOIN pg_catalog.pg_namespace nsp ON nsp.oid = connamespace
        WHERE nsp.nspname = 'events'
            AND rel.relname = 'OrganDonationRegistrationGetMetric'
            AND con.conname = 'organdonationregistrationgetmetric_auditid_unique'
    )
    THEN
        ALTER TABLE events."OrganDonationRegistrationGetMetric"
            ADD CONSTRAINT organdonationregistrationgetmetric_auditid_unique UNIQUE ("AuditId");
    END IF;
    
END 
$$;