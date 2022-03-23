
CREATE TABLE IF NOT EXISTS reports."MonthlyUsageDelta"
(
    "Month" timestamp without time zone,
    "P5NewAppUsers" numeric,
    "AcceptedTermsAndConditions" numeric,
    "P9VerifiedNHSAppUsers" numeric,
    "Logins" numeric,
    "UsersLogin" bigint,
    "RecordViews" numeric,
    "UsersRecordViews" integer,
    "Prescriptions" numeric,
    "UsersPrescriptions" integer,
    "ODRegistrations" numeric,
    "UsersODRegistrations" integer,
    "NomPharmacy" numeric,
    "UsersNomPharmacy" integer,
    "AppointmentsBooked" integer,
    "UsersAppointmentsBooked" integer
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

CALL perms.apply_etl_select_permissions('reports', 'MonthlyUsageDelta');
GRANT INSERT ON reports."MonthlyUsageDelta" TO "GRP-AzureNhsApp-AnalyticsAdmins-DL";
