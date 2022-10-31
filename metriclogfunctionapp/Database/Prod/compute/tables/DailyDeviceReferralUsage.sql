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
    CALL perms.apply_compute_table_permissions('compute', 'DailyDeviceReferralUsage');
END
$$;