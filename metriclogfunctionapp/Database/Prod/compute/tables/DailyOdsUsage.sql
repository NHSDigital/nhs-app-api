CREATE TABLE IF NOT EXISTS "compute"."DailyOdsUsage"(
                                                     "Date" date NOT NULL,
                                                     "OdsCode" varchar (6) NOT NULL,
                                                     "P5NewAppUsers" int,
                                                     "AcceptedTermsAndConditions" int,
                                                     "P9VerifiedNHSAppUsers" int,
                                                     "Logins" int,
                                                     "RecordViews" int,
                                                     "Prescriptions" int,
                                                     "NomPharmacy" int,
                                                     "AppointmentsBooked" int,
                                                     "AppointmentsCancelled" int,
                                                     "ODRegistrations" int,
                                                     "ODWithdrawals" int,
                                                     "ODUpdates" int,
                                                     "ODLookups" int,
                                                     "RecordViewsDCR" INT DEFAULT 0,
                                                     "RecordViewsSCR" INT DEFAULT 0);

CREATE UNIQUE INDEX IF NOT EXISTS idx_date_odscode_dailyodsusage ON "compute"."DailyOdsUsage"("Date", "OdsCode");

CALL perms.apply_compute_table_permissions('compute', 'DailyOdsUsage');