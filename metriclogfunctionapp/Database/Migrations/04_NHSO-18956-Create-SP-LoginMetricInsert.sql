DROP PROCEDURE IF EXISTS events.LoginMetricInsert;

CREATE PROCEDURE events.LoginMetricInsert(
    loginTimestamp timestamp with time zone,
    odsCode character varying(6),
    loginId character varying,
    proofLevel character varying(2),
    referrer text,
    sessionId character varying(36),
    auditId character varying(36))
AS $$

BEGIN

INSERT INTO "events"."LoginMetric" ("Timestamp", "OdsCode", "LoginId", "ProofLevel", "Referrer", "SessionId", "AuditId")
VALUES ( loginTimestamp, odsCode, loginId, proofLevel, referrer, sessionId, auditId)
ON CONFLICT ON CONSTRAINT loginmetric_auditid_unique
DO NOTHING;

END;

$$ LANGUAGE plpgsql;