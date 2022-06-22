CREATE OR REPLACE PROCEDURE events.OrganDonationRegistrationGetMetricInsert (
    organDonationRegistrationGetMetricTimestamp timestamp with time zone,
    sessionId character varying(36),
    auditId character varying(36))
AS $$

BEGIN
    INSERT INTO events."OrganDonationRegistrationGetMetric" ("Timestamp", "SessionId", "AuditId")
    VALUES (organDonationRegistrationGetMetricTimestamp, sessionId, auditId)
        ON CONFLICT ON CONSTRAINT organdonationregistrationgetmetric_auditid_unique
        DO NOTHING;
        
END;

$$ LANGUAGE plpgsql;