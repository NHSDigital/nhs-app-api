CREATE UNIQUE INDEX IF NOT EXISTS idx_date_odscode ON "compute"."DailyOdsTransactions"("Date", "OdsCode");

CREATE INDEX IF NOT EXISTS idx_date ON "compute"."DailyOdsTransactions"("Date");