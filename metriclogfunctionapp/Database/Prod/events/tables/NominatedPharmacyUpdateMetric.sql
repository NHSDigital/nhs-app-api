DO $$
    BEGIN

        CREATE TABLE IF NOT EXISTS events."NominatedPharmacyUpdateMetric" (
            "Timestamp" timestamp with time zone NOT NULL,
            "SessionId" character varying(36) NOT NULL
        );

        CALL perms.apply_etl_table_permissions('events', 'NominatedPharmacyUpdateMetric');

        CREATE INDEX IF NOT EXISTS NominatedPharmacyUpdateMetric_Timestamp_SessionId_idx on events."NominatedPharmacyUpdateMetric" ("Timestamp", "SessionId");
        CREATE INDEX IF NOT EXISTS NominatedPharmacyUpdateMetric_date_idx on events."NominatedPharmacyUpdateMetric" ("Timestamp");

        ALTER TABLE events."NominatedPharmacyUpdateMetric"
            ADD COLUMN IF NOT EXISTS "AuditId" character varying (36) NULL;

        IF NOT EXISTS (
            SELECT con.*
            FROM pg_catalog.pg_constraint con
                     INNER JOIN pg_catalog.pg_class rel ON rel.oid = con.conrelid
                     INNER JOIN pg_catalog.pg_namespace nsp ON nsp.oid = connamespace
            WHERE nsp.nspname = 'events'
              AND rel.relname = 'NominatedPharmacyUpdateMetric'
              AND con.conname = 'nominatedpharmacyupdatemetric_auditid_unique'
            )
        THEN
            ALTER TABLE events."NominatedPharmacyUpdateMetric"
                ADD CONSTRAINT nominatedpharmacyupdatemetric_auditid_unique UNIQUE ("AuditId");
        END IF;

    END
$$;