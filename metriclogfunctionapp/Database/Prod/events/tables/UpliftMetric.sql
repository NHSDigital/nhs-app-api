
CREATE TABLE IF NOT EXISTS events."UpliftMetric"
(
    "Timestamp" timestamp(6) with time zone NOT NULL,
    "OdsCode" character varying COLLATE pg_catalog."default",
    "LoginId" character varying(36) COLLATE pg_catalog."default" NOT NULL,
    "ProofLevel" character varying(2) COLLATE pg_catalog."default" NOT NULL,
    "Action" character varying COLLATE pg_catalog."default" NOT NULL
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

CALL perms.apply_etl_table_permissions('events', 'UpliftMetric');
