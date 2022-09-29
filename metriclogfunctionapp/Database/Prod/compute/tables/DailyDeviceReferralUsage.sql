DO $$
BEGIN

CREATE TABLE IF NOT EXISTS compute."DailyDeviceReferralUsage"
(
    "Date" date,
    "DeviceOS" character varying COLLATE pg_catalog."default",
    "Referral" character varying COLLATE pg_catalog."default",
    "Users" integer,
    "Logins" integer,
    "RecordViewsDCR" integer,
    "RecordViewsSCR" integer,
    "Prescriptions" integer,
    "NomPharmacy" integer,
    "AppointmentsBooked" integer,
    "AppointmentsCancelled" integer,
    "ODRegistrations" integer,
    "ODWithdrawals" integer,
    CONSTRAINT uniq_pkey UNIQUE ("Date", "DeviceOS", "Referral")
);
    ALTER TABLE compute."DailyDeviceReferralUsage"
         ADD COLUMN IF NOT EXISTS "ReferralOrigin" character varying COLLATE pg_catalog."default";
    IF NOT EXISTS (
        SELECT con.*
        FROM pg_catalog.pg_constraint con
            INNER JOIN pg_catalog.pg_class rel ON rel.oid = con.conrelid
            INNER JOIN pg_catalog.pg_namespace nsp ON nsp.oid = connamespace
        WHERE nsp.nspname = 'compute'
            AND rel.relname = 'DailyDeviceReferralUsage'
            AND con.conname = 'uniq_pkey'
    )
    THEN
        ALTER TABLE compute."DailyDeviceReferralUsage"
            ADD CONSTRAINT uniq_pkey UNIQUE ("Date", "DeviceOS", "Referral", "ReferralOrigin");
    END IF;
    CALL perms.apply_compute_table_permissions('compute', 'DailyDeviceReferralUsage'); 
END 
$$;