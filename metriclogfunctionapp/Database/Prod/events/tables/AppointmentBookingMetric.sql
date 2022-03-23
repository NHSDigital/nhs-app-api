
CREATE TABLE IF NOT EXISTS events."AppointmentBookingMetric" (
    "OdsCode" character varying NOT NULL,
    "SessionName" text,
    "SlotType" text,
    "StatusCode" INT,
    "Date" DATE
);

CALL perms.apply_etl_table_permissions('events', 'AppointmentBookingMetric');

CREATE INDEX IF NOT EXISTS AppointmentBookingMetric_date_idx on events."AppointmentBookingMetric" ("Date");
