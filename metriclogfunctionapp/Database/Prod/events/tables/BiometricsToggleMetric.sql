DO $$
BEGIN

    CREATE TABLE IF NOT EXISTS events."BiometricsToggleMetric" (
        "SessionId" character varying (36) NULL,
        "Timestamp" timestamp with time zone NOT NULL,
        "BiometricsToggle" character varying (3) NULL,
        "AuditId" character varying (36) NULL
    );

    CALL perms.apply_etl_table_permissions('events', 'BiometricsToggleMetric');

END$$;