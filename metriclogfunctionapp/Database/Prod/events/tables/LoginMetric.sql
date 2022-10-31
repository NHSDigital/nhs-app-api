DO $$
BEGIN
    CREATE TABLE IF NOT EXISTS events."LoginMetric" (
        "Timestamp" timestamp with time zone NOT NULL,
        "OdsCode" character varying(6) NOT NULL,
        "LoginId" character varying NOT NULL,
        "ProofLevel" character varying(2) NOT NULL,
        "LoginEventId" character varying(36) NULL,
        "Referrer" text NULL,
        "SessionId" character varying(36) NULL,
        "AuditId" character varying(36) NULL
    );

    CALL perms.apply_etl_table_permissions('events', 'LoginMetric');
END$$;