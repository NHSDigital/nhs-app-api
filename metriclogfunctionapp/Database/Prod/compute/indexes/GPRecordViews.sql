-- Table: compute.GPRecordViews
DO $$
BEGIN

-- Index: GPRecordViews_Date_idx
CREATE INDEX IF NOT EXISTS "GPRecordViews_Date_idx"
    ON compute."GPRecordViews" USING btree
    ("Date" ASC NULLS LAST);

-- Index: GPRecordViews_OdsCode_idx
CREATE INDEX IF NOT EXISTS "GPRecordViews_OdsCode_idx"
    ON compute."GPRecordViews" USING btree
    ("OdsCode" COLLATE pg_catalog."default" ASC NULLS LAST);

-- Index: GPRecordViews_Supplier_idx
CREATE INDEX IF NOT EXISTS "GPRecordViews_Supplier_idx"
    ON compute."GPRecordViews" USING btree
    ("Supplier" COLLATE pg_catalog."default" ASC NULLS LAST);

END$$;