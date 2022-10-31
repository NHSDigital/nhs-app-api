CREATE INDEX IF NOT EXISTS FirstLogins_FirstP5LoginTimestamp_idx on "compute"."FirstLogins" ("FirstP5LoginTimestamp");
CREATE INDEX IF NOT EXISTS FirstLogins_FirstP9LoginTimestamp_idx on "compute"."FirstLogins" ("FirstP9LoginTimestamp");
CREATE INDEX IF NOT EXISTS FirstLogins_ConsentTimestamp_idx on "compute"."FirstLogins" ("ConsentTimestamp");