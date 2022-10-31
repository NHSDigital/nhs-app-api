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
END$$;