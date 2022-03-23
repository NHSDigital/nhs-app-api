DROP PROCEDURE IF EXISTS events.ConsentMetricInsert;

CREATE PROCEDURE events.ConsentMetricInsert(
    consentTimestamp timestamp with time zone,
    sessionId character varying(36),
    auditId character varying(36),
    loginId character varying,
    proofLevel character varying(2))
AS $$

BEGIN

INSERT INTO "events"."ConsentMetric" ("Timestamp", "SessionId", "AuditId", "LoginId", "ProofLevel")
VALUES ( consentTimestamp, sessionId, auditId, loginId, proofLevel)
ON CONFLICT ON CONSTRAINT consentmetric_auditid_unique
DO NOTHING;

END;

$$ LANGUAGE plpgsql;