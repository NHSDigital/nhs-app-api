DO $$
BEGIN
    CREATE TABLE IF NOT EXISTS events."PrescriptionOrdersMetric" (
    "Timestamp" timestamp with time zone NOT NULL,
    "SessionId" character varying(36) NOT NULL
);

    ALTER TABLE events."PrescriptionOrdersMetric"
        ADD COLUMN IF NOT EXISTS "AuditId" character varying (36) NULL;
        
        IF NOT EXISTS (
            SELECT con.*
            FROM pg_catalog.pg_constraint con
                INNER JOIN pg_catalog.pg_class rel ON rel.oid = con.conrelid
                INNER JOIN pg_catalog.pg_namespace nsp ON nsp.oid = connamespace
            WHERE nsp.nspname = 'events'
                AND rel.relname = 'PrescriptionOrdersMetric'
                AND con.conname = 'prescriptionordersmetric_auditid_unique'
        )
        THEN
            ALTER TABLE events."PrescriptionOrdersMetric"
                ADD CONSTRAINT prescriptionordersmetric_auditid_unique UNIQUE ("AuditId");
        END IF;


    CALL perms.apply_etl_table_permissions('events', 'PrescriptionOrdersMetric');

    CREATE INDEX IF NOT EXISTS PrescriptionOrdersMetric_SessionId_idx on events."PrescriptionOrdersMetric" ("SessionId");
    CREATE INDEX IF NOT EXISTS PrescriptionOrdersMetric_date_idx on events."PrescriptionOrdersMetric" ("Timestamp");
END
$$;
