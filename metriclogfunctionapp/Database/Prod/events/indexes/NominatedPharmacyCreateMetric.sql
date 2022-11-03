DO $$
    BEGIN

        CREATE INDEX IF NOT EXISTS NominatedPharmacyCreateMetric_Timestamp_SessionId_idx on events."NominatedPharmacyCreateMetric" ("Timestamp", "SessionId");
        CREATE INDEX IF NOT EXISTS NominatedPharmacyCreateMetric_date_idx on events."NominatedPharmacyCreateMetric" ("Timestamp");

        CREATE INDEX IF NOT EXISTS NominatedPharmacyCreateMetric_SessionId_idx
            ON events."NominatedPharmacyCreateMetric" ("SessionId");

        IF NOT EXISTS (
            SELECT con.*
            FROM pg_catalog.pg_constraint con
                     INNER JOIN pg_catalog.pg_class rel ON rel.oid = con.conrelid
                     INNER JOIN pg_catalog.pg_namespace nsp ON nsp.oid = connamespace
            WHERE nsp.nspname = 'events'
              AND rel.relname = 'NominatedPharmacyCreateMetric'
              AND con.conname = 'nominatedpharmacycreatemetric_auditid_unique'
            )
        THEN
            ALTER TABLE events."NominatedPharmacyCreateMetric"
                ADD CONSTRAINT nominatedpharmacycreatemetric_auditid_unique UNIQUE ("AuditId");
        END IF;

    END
$$;
