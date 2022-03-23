
CREATE TABLE IF NOT EXISTS public."Transactions" (
    "TransactionDate" timestamp without time zone NOT NULL,
    "OdsCode" text NOT NULL,
    "Provider" text,
    "AppointmentsBooked" integer NOT NULL,
    "AppointmentsCancelled" integer NOT NULL,
    "RepeatsOrdered" integer NOT NULL,
    "RecordViews" integer NOT NULL,
    "OdrRegistrations" integer NOT NULL,
    "OdrUpdates" integer NOT NULL,
    "OdrWithdrawals" integer NOT NULL,
    "NominatedPharmacyChanges" integer NOT NULL,
    "NdopStarts" integer NOT NULL,
    "Logins" integer NOT NULL,
    "TimeoutCount" integer NOT NULL,
    "Registrations" integer NOT NULL,
    "RegistrationsPYI" integer NOT NULL,
    "OdrLookups" integer DEFAULT 0 NOT NULL
);

CALL perms.apply_legacy_insert_update_table_permissions('public', 'Transactions');
