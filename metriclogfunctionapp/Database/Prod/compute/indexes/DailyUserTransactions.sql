CREATE UNIQUE INDEX IF NOT EXISTS idx_date_loginid ON "compute"."DailyUserTransactions"("Date", "LoginId");

CREATE INDEX IF NOT EXISTS idx_date ON "compute"."DailyUserTransactions"("Date");
DROP INDEX IF EXISTS "compute".dailyusertransactions_date_recordviewsdcr_idx;
DROP INDEX IF EXISTS  "compute".dailyusertransactions_date_recordviewsscr_idx;
CREATE INDEX IF NOT EXISTS dailyusertransactions_date_loginid_idx ON "compute"."DailyUserTransactions" ("Date","LoginId");