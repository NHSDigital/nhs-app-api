CREATE TABLE IF NOT EXISTS "compute"."DailyOdsTransactions" (
                                                                 "Date" timestamp with time zone NOT NULL,
                                                                 "OdsCode" varchar (6) NOT NULL,
                                                                 "Logins" int,
                                                                 "RecordViews" int,
                                                                 "Prescriptions" int,
                                                                 "ODRegistrations" int,
                                                                 "NomPharmacyUpdate" int,
                                                                 "NomPharmacyCreate" int,
                                                                 "AppointmentsBooked" int,
                                                                 "AppointmentsCancelled" int,
                                                                 "ODWithdrawals" INT DEFAULT 0,
                                                                 "ODLookups" INT DEFAULT 0,
                                                                 "ODUpdates" INT DEFAULT 0,
                                                                 "RecordViewsDCR" INT DEFAULT 0,
                                                                 "RecordViewsSCR" INT DEFAULT 0);

CALL perms.apply_compute_table_permissions('compute', 'DailyOdsTransactions');