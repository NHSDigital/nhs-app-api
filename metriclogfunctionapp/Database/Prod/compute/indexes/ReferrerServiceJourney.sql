DO $$
BEGIN
    IF NOT EXISTS (
        SELECT con.*
        FROM pg_catalog.pg_constraint con
            INNER JOIN pg_catalog.pg_class rel ON rel.oid = con.conrelid
            INNER JOIN pg_catalog.pg_namespace nsp ON nsp.oid = connamespace
        WHERE nsp.nspname = 'compute'
            AND rel.relname = 'ReferrerServiceJourney'
            AND con.conname = 'referrerservicejourney_date_referrerid_unique'
    )
    THEN
        ALTER TABLE compute."ReferrerServiceJourney"
            ADD CONSTRAINT referrerservicejourney_date_referrerid_unique UNIQUE ("Date", "ReferrerId");
    END IF;
END$$;