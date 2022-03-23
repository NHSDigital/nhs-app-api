
CREATE TABLE IF NOT EXISTS ref."ccg_stp_region_lookup_2020"
(
    "ccgcd" character varying COLLATE pg_catalog."default",
    "ccg_code" character varying COLLATE pg_catalog."default",
    "ccg_name" character varying COLLATE pg_catalog."default",
    "stp_code" character varying COLLATE pg_catalog."default",
    "stpcdh" character varying COLLATE pg_catalog."default",
    "stp_name" character varying COLLATE pg_catalog."default",
    "nhsercd" character varying COLLATE pg_catalog."default",
    "nhsercdh" character varying COLLATE pg_catalog."default",
    "region_name" character varying COLLATE pg_catalog."default",
    "fid" integer
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

CALL perms.apply_etl_select_permissions('ref', 'ccg_stp_region_lookup_2020');
