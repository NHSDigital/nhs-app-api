DROP PROCEDURE IF EXISTS events.OrganDonationRegistrationUpdateMetricInsert;
CREATE PROCEDURE events.OrganDonationRegistrationUpdateMetricInsert (
    organDonationRegistrationUpdateMetricTimestamp timestamp with time zone,
    sessionId character varying(36),
    auditId character varying(36))
    AS $$
BEGIN
INSERT INTO "events"."OrganDonationRegistrationUpdateMetric" ("Timestamp", "SessionId", "AuditId")
VALUES (organDonationRegistrationUpdateMetricTimestamp, sessionId, auditId)
    ON CONFLICT ON CONSTRAINT organDonationRegistrationUpdateMetric_auditid_unique
    DO NOTHING;
END;
$$ LANGUAGE plpgsql;