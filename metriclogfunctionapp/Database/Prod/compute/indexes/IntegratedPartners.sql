DO $$
BEGIN

    IF NOT EXISTS (
        SELECT con.*
        FROM pg_catalog.pg_constraint con
            INNER JOIN pg_catalog.pg_class rel ON rel.oid = con.conrelid
            INNER JOIN pg_catalog.pg_namespace nsp ON nsp.oid = connamespace
        WHERE nsp.nspname = 'compute'
            AND rel.relname = 'IntegratedPartners'
            AND con.conname = 'integratedpartners_date_odscode_provider_jumpoff_unique'
    )
    THEN
        ALTER TABLE compute."IntegratedPartners"
            ADD CONSTRAINT integratedpartners_date_odscode_provider_jumpoff_unique UNIQUE ("Date", "OdsCode", "Provider", "JumpOff");
    END IF;

    CREATE INDEX IF NOT EXISTS integratedpartners_date_idx on compute."IntegratedPartners" ("Date");

END$$;
