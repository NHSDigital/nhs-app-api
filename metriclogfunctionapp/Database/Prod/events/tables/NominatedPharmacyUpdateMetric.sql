DO $$
    BEGIN

        CREATE TABLE IF NOT EXISTS events."NominatedPharmacyUpdateMetric" (
            "Timestamp" timestamp with time zone NOT NULL,
            "SessionId" character varying(36) NOT NULL
        );

        CALL perms.apply_etl_table_permissions('events', 'NominatedPharmacyUpdateMetric');

        ALTER TABLE events."NominatedPharmacyUpdateMetric"
            ADD COLUMN IF NOT EXISTS "AuditId" character varying (36) NULL;

    END
$$;