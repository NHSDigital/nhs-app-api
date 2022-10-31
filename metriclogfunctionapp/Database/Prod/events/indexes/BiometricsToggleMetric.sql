DO $$
BEGIN

    IF NOT EXISTS (
        SELECT con.*
        FROM pg_catalog.pg_constraint con
            INNER JOIN pg_catalog.pg_class rel ON rel.oid = con.conrelid
            INNER JOIN pg_catalog.pg_namespace nsp ON nsp.oid = connamespace
        WHERE nsp.nspname = 'events'
            AND rel.relname = 'BiometricsToggleMetric'
            AND con.conname = 'biometricstogglemetric_auditid_unique'
    )
    THEN
        ALTER TABLE events."BiometricsToggleMetric"
            ADD CONSTRAINT biometricstogglemetric_auditid_unique UNIQUE ("AuditId");
    END IF;

END$$;