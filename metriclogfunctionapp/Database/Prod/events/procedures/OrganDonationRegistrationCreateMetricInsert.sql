DROP PROCEDURE IF EXISTS events.OrganDonationRegistrationCreateMetricInsert;
CREATE PROCEDURE events.OrganDonationRegistrationCreateMetricInsert (
    organDonationRegistrationCreateMetricTimestamp timestamp with time zone,
    sessionId character varying(36),
    auditId character varying(36))
    AS $$
BEGIN
INSERT INTO "events"."OrganDonationRegistrationCreateMetric" ("Timestamp", "SessionId", "AuditId")
VALUES (organDonationRegistrationCreateMetricTimestamp, sessionId, auditId)
    ON CONFLICT ON CONSTRAINT organDonationRegistrationCreateMetric_auditid_unique
    DO NOTHING;
END;
$$ LANGUAGE plpgsql;