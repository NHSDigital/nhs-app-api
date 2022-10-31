DO $$
BEGIN
    IF NOT EXISTS (
        SELECT con.*
        FROM pg_catalog.pg_constraint con
            INNER JOIN pg_catalog.pg_class rel ON rel.oid = con.conrelid
            INNER JOIN pg_catalog.pg_namespace nsp ON nsp.oid = connamespace
        WHERE nsp.nspname = 'compute'
            AND rel.relname = 'DailyDeviceReferralUsage'
            AND con.conname = 'uniq_pkey'
    )
    THEN
        ALTER TABLE compute."DailyDeviceReferralUsage"
            ADD CONSTRAINT uniq_pkey UNIQUE ("Date", "DeviceOS", "Referral", "ReferralOrigin");
    END IF;
END
$$;