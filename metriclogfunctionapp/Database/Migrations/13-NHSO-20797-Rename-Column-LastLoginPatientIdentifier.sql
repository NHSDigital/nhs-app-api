DO $$
BEGIN
    IF EXISTS (SELECT FROM information_schema.columns 
               WHERE table_schema='events'
                 AND table_name='LastLoginPatientIdentifier'
                 AND column_name='LastLoginTimestamp') THEN

      ALTER TABLE events."LastLoginPatientIdentifier" 
      RENAME COLUMN "LastLoginTimestamp" TO "Timestamp";

      DROP INDEX IF EXISTS LastLoginPatientIdentifier_Timestamp_Idx;
      DROP INDEX IF EXISTS LastLoginPatientIdentifier_NhsNumber_idx1;

    END IF;
END$$;
