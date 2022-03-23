DROP INDEX IF EXISTS idx_date_loginid;

CREATE TABLE IF NOT EXISTS "compute"."DailyUserTransactions" (
                                                                 "LoginId" character varying NOT NULL,
                                                                 "Date" timestamp with time zone NOT NULL,
                                                                 "Logins" int,
                                                                 "RecordViews" int,
                                                                 "Prescriptions" int,
                                                                 "ODRegistrations" int,
                                                                 "NomPharmacyUpdate" int,
                                                                 "NomPharmacyCreate" int,
                                                                 "AppointmentsBooked" int,
                                                                 "ODWithdrawals" int,
                                                                 "ODUpdates" int,
                                                                 "ODLookups" int,
                                                                 "AppointmentsCancelled" int DEFAULT 0,
                                                                 "RecordViewsDCR" INT DEFAULT 0,
                                                                 "RecordViewsSCR" INT DEFAULT 0);

CREATE UNIQUE INDEX IF NOT EXISTS idx_date_loginid ON "compute"."DailyUserTransactions"("Date", "LoginId");

CREATE INDEX IF NOT EXISTS idx_date ON "compute"."DailyUserTransactions"("Date");

CALL perms.apply_compute_table_permissions('compute', 'DailyUserTransactions');