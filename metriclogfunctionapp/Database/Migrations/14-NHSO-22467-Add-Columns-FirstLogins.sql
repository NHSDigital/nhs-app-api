ALTER TABLE compute."FirstLogins"
            ADD COLUMN IF NOT EXISTS "OptedIn" character varying(1) NULL,
            ADD COLUMN IF NOT EXISTS "OptedInTimestamp" timestamp with time zone NULL;

CREATE INDEX IF NOT EXISTS FirstLogins_OptedInTimestamp_idx on "compute"."FirstLogins" ("OptedInTimestamp");
