
CREATE TABLE IF NOT EXISTS events."AppointmentCancellingMetric" (
    "OdsCode" character varying NOT NULL,
    "SessionName" text,
    "SlotType" text,
    "StatusCode" INT,
    "Date" DATE
);

CALL perms.apply_etl_table_permissions('events', 'AppointmentCancellingMetric');

CREATE INDEX IF NOT EXISTS AppointmentCancellingMetric_date_idx on events."AppointmentCancellingMetric" ("Date");
