
CREATE TABLE IF NOT EXISTS reports."WeeklyUsageDelta"
(
    "Week" timestamp without time zone,
    "P5NewAppUsers" numeric,
    "AcceptedTermsAndConditions" numeric,
    "P9VerifiedNHSAppUsers" numeric,
    "Logins" numeric,
    "UsersLogin" bigint,
    "RecordViews" numeric,
    "UsersRecordViews" bigint,
    "Prescriptions" numeric,
    "UsersPrescriptions" bigint,
    "ODRegistrations" numeric,
    "UsersODRegistrations" bigint,
    "NomPharmacy" numeric,
    "UsersNomPharmacy" bigint,
    "AppointmentsBooked" integer,
    "UsersAppointmentsBooked" integer
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

CALL perms.apply_etl_select_permissions('reports', 'WeeklyUsageDelta');
GRANT INSERT ON reports."WeeklyUsageDelta" TO "GRP-AzureNhsApp-AnalyticsAdmins-DL";
