DO $$
    BEGIN

        CREATE TABLE IF NOT EXISTS events."NominatedPharmacyCreateMetric" (
            "Timestamp" timestamp with time zone NOT NULL,
            "SessionId" character varying(36) NOT NULL
        );

        CALL perms.apply_etl_table_permissions('events', 'NominatedPharmacyCreateMetric');

        ALTER TABLE events."NominatedPharmacyCreateMetric"
            ADD COLUMN IF NOT EXISTS "AuditId" character varying (36) NULL;

    END
$$;
