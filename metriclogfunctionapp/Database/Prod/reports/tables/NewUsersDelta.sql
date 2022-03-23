
CREATE TABLE IF NOT EXISTS reports."NewUsersDelta"
(
    "Date" timestamp without time zone,
    "P5NewAppUsers" numeric,
    "AcceptedTermsAndConditions" numeric,
    "P9VerifiedNHSAppUsers" numeric
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

CALL perms.apply_etl_select_permissions('reports', 'NewUsersDelta');
GRANT INSERT ON reports."NewUsersDelta" TO "GRP-AzureNhsApp-AnalyticsAdmins-DL";
