
CREATE TABLE IF NOT EXISTS reports."SilverIntegrationHistoric"
(
    "Date" date,
    "STP" character varying COLLATE pg_catalog."default",
    "CCG" character varying COLLATE pg_catalog."default",
    "PracticeName" character varying COLLATE pg_catalog."default",
    "OdsCode" character varying COLLATE pg_catalog."default",
    "Provider" character varying COLLATE pg_catalog."default",
    "Access" text COLLATE pg_catalog."default",
    "JumpOff" character varying COLLATE pg_catalog."default",
    "Clicks" integer,
    "Users" integer
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

CALL perms.apply_etl_select_permissions('reports', 'SilverIntegrationHistoric');
