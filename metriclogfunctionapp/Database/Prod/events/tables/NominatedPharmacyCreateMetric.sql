
CREATE TABLE IF NOT EXISTS events."NominatedPharmacyCreateMetric" (
    "Timestamp" timestamp with time zone NOT NULL,
    "SessionId" character varying(36) NOT NULL
);

CALL perms.apply_etl_table_permissions('events', 'NominatedPharmacyCreateMetric');

CREATE INDEX IF NOT EXISTS NominatedPharmacyCreateMetric_Timestamp_SessionId_idx on events."NominatedPharmacyCreateMetric" ("Timestamp", "SessionId");
CREATE INDEX IF NOT EXISTS NominatedPharmacyCreateMetric_date_idx on events."NominatedPharmacyCreateMetric" ("Timestamp");
