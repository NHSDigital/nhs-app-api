
CREATE TABLE IF NOT EXISTS public."eConsultation_Transactions"
(
    "Date" timestamp without time zone NOT NULL,
    "OdsCode" text COLLATE pg_catalog."default" NOT NULL,
    "Provider" text COLLATE pg_catalog."default",
    "Started" integer NOT NULL,
    "Completed" integer NOT NULL,
    "Errors" integer NOT NULL,
    "AdminRoute" integer NOT NULL,
    "TriageRoute" integer NOT NULL
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

CALL perms.apply_legacy_insert_update_table_permissions('public', 'eConsultation_Transactions');
