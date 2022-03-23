
CREATE TABLE IF NOT EXISTS outcomes."GP-active"
(
    "OrgId" character varying COLLATE pg_catalog."default" NOT NULL,
    "Name" character varying COLLATE pg_catalog."default",
    "AddrLn1" character varying COLLATE pg_catalog."default",
    "PostCode" character varying COLLATE pg_catalog."default",
    "Status" character varying COLLATE pg_catalog."default",
    "gpPractice_status_string" character varying COLLATE pg_catalog."default",
    "gpPractice_status" character varying COLLATE pg_catalog."default",
    "orgRecordClass" character varying COLLATE pg_catalog."default",
    "ccgCode" character varying COLLATE pg_catalog."default",
    "LastChangeDate" character varying COLLATE pg_catalog."default",
    "PrimaryRoleDescription" character varying COLLATE pg_catalog."default"
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

CALL perms.apply_etl_select_permissions('outcomes', 'GP-active');
