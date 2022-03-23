
CREATE TABLE IF NOT EXISTS public."Proxy_Transactions"
(
    "Date" timestamp without time zone NOT NULL,
    "PracticeCode" text COLLATE pg_catalog."default" NOT NULL,
    "Supplier" text COLLATE pg_catalog."default",
    "AppointmentsBookedProxy" integer NOT NULL,
    "AppointmentsBookedNonProxy" integer NOT NULL,
    "RepeatsOrderedProxy" integer NOT NULL,
    "RepeatsOrderedNonProxy" integer NOT NULL,
    "RecordViewsProxy" integer NOT NULL,
    "RecordViewsNonProxy" integer NOT NULL
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

CALL perms.apply_legacy_insert_update_table_permissions('public', 'Proxy_Transactions');
