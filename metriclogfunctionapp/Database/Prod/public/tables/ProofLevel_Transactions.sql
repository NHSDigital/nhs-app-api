
CREATE TABLE IF NOT EXISTS public."ProofLevel_Transactions"
(
    "Date" timestamp without time zone NOT NULL,
    "PracticeCode" text COLLATE pg_catalog."default" NOT NULL,
    "P5NewAppUsers" integer NOT NULL,
    "P9SuccessfulUplifts" integer NOT NULL,
    "P9AttemptedUplifts" integer NOT NULL DEFAULT 0,
    "AcceptedTermsAndConditions" integer NOT NULL DEFAULT 0
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

CALL perms.apply_legacy_insert_update_table_permissions('public', 'ProofLevel_Transactions');
