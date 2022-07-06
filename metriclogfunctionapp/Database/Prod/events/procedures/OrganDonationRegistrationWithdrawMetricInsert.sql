CREATE OR REPLACE PROCEDURE events.OrganDonationRegistrationWithdrawMetricInsert (
    organDonationRegistrationWithdrawMetricTimestamp timestamp with time zone,
    sessionId character varying(36),
    auditId character varying(36))
AS $$

BEGIN
    INSERT INTO events."OrganDonationRegistrationWithdrawMetric" ("Timestamp", "SessionId", "AuditId")
    VALUES (organDonationRegistrationWithdrawMetricTimestamp, sessionId, auditId)
        ON CONFLICT ON CONSTRAINT organdonationregistrationwithdrawmetric_auditid_unique
        DO NOTHING;
END;

$$ LANGUAGE plpgsql;