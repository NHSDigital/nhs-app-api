DO $$
BEGIN
    IF EXISTS (SELECT FROM information_schema.columns
               WHERE table_schema='events'
                 AND table_name='SilverIntegrationJumpOffMetric'
                 AND column_name='SessionId'
                 AND data_type='character varying') THEN

    ALTER TABLE events."SilverIntegrationJumpOffMetric"
        ALTER COLUMN "SessionId" TYPE character varying(36);
    END IF;
END
$$;