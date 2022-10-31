DO $$
BEGIN
    CREATE TABLE IF NOT EXISTS events."ConsentMetric" (
        "Timestamp" timestamp with time zone NOT NULL,
        "OdsCode" character varying(6) NULL,
        "LoginId" character varying(36) NOT NULL,
        "ProofLevel" character varying(2) NOT NULL,
        "SessionId" character varying(36) NULL,
        "AuditId" character varying(36) NULL
    );

    CALL perms.apply_etl_table_permissions('events', 'ConsentMetric');
END$$;