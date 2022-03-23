DROP PROCEDURE IF EXISTS events.WebIntegrationReferralsInsert;

CREATE PROCEDURE events.WebIntegrationReferralsInsert(
    referralTimestamp timestamp with time zone,
    referrer text,
    sessionId character varying(36),
    auditId character varying(36))
AS $$

BEGIN

    INSERT INTO "events"."WebIntegrationReferrals" ("Timestamp", "Referrer", "SessionId", "AuditId")
    VALUES (referralTimestamp, referrer, sessionId, auditId)
    ON CONFLICT ON CONSTRAINT webintegrationrefferrals_auditid_unique
        DO NOTHING;

END;

$$ LANGUAGE plpgsql;