
CREATE TABLE IF NOT EXISTS events."OrganDonationRegistrationUpdateMetric" (
    "Timestamp" timestamp with time zone NOT NULL,
    "SessionId" character varying(36) NOT NULL
);

CALL perms.apply_etl_table_permissions('events', 'OrganDonationRegistrationUpdateMetric');
