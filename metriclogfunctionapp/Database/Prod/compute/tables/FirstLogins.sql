CREATE TABLE IF NOT EXISTS "compute"."FirstLogins" (
    "LoginId" text NOT NULL UNIQUE PRIMARY KEY,
    "FirstP5LoginDate" date,
    "FirstP9LoginDate" date,
    "ConsentDate" date,
    "ConsentProofLevel" text,
    "FirstP5LoginTimestamp" timestamp with time zone,
    "FirstP9LoginTimestamp" timestamp with time zone,
    "ConsentTimestamp" timestamp with time zone,
    "LatestOdsCode" character varying(6),
    "LatestProofLevel" character varying(2),
    "LatestLoginTimestamp" timestamp with time zone,
    "SingleLoginFlag" character varying(1)
);

CALL perms.apply_compute_table_permissions('compute', 'FirstLogins');