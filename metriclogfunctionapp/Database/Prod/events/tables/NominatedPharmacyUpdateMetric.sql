
CREATE TABLE IF NOT EXISTS events."NominatedPharmacyUpdateMetric" (
    "Timestamp" timestamp with time zone NOT NULL,
    "SessionId" character varying(36) NOT NULL
);

CALL perms.apply_etl_table_permissions('events', 'NominatedPharmacyUpdateMetric');

CREATE INDEX IF NOT EXISTS NominatedPharmacyUpdateMetric_Timestamp_SessionId_idx on events."NominatedPharmacyUpdateMetric" ("Timestamp", "SessionId");
CREATE INDEX IF NOT EXISTS NominatedPharmacyUpdateMetric_date_idx on events."NominatedPharmacyUpdateMetric" ("Timestamp");
