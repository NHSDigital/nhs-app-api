DO $$
BEGIN
    CREATE TABLE IF NOT EXISTS compute."ReferrerServiceJourney" (
        "Date" date,
        "ReferrerId" text NOT NULL,
        "RecordViews" integer DEFAULT 0,
        "Prescriptions" integer DEFAULT 0,
        "OdRegistrations" integer DEFAULT 0,
        "OdWithdrawals" integer DEFAULT 0,
        "OdUpdates" integer DEFAULT 0,
        "OdLookups" integer DEFAULT 0,
        "NomPharmacyUpdate" integer DEFAULT 0,
        "NomPharmacyCreate" integer DEFAULT 0,
        "AppointmentsBooked" integer DEFAULT 0,
        "AppointmentsCancelled" integer DEFAULT 0,
        "RecordViewsDcr" integer DEFAULT 0,
        "RecordViewsScr" integer DEFAULT 0,
        "SilverIntegrationJumpOffs" integer DEFAULT 0,
        "CovidPassJumpOffs" integer DEFAULT 0
    );
    CALL perms.apply_compute_table_permissions('compute', 'ReferrerServiceJourney');

    IF NOT EXISTS (
        SELECT con.*
        FROM pg_catalog.pg_constraint con
            INNER JOIN pg_catalog.pg_class rel ON rel.oid = con.conrelid
            INNER JOIN pg_catalog.pg_namespace nsp ON nsp.oid = connamespace
        WHERE nsp.nspname = 'compute'
            AND rel.relname = 'ReferrerServiceJourney'
            AND con.conname = 'referrerservicejourney_date_referrerid_unique'
    )
    THEN
        ALTER TABLE compute."ReferrerServiceJourney"
            ADD CONSTRAINT referrerservicejourney_date_referrerid_unique UNIQUE ("Date", "ReferrerId");
    END IF;
END$$;