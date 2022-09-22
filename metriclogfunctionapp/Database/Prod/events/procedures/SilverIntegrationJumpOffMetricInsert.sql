DROP PROCEDURE IF EXISTS events.SilverIntegrationJumpOffMetricInsert;

CREATE PROCEDURE events.SilverIntegrationJumpOffMetricInsert (
    silverIntegrationTimestamp timestamp with time zone,
    sessionId character varying(36),
    providerId text,
    providerName text,
    jumpOffId text,
    auditId character varying(36))
    AS $$

BEGIN
INSERT INTO "events"."SilverIntegrationJumpOffMetric" ("Timestamp", "SessionId", "ProviderId", "ProviderName", "JumpOffId", "AuditId")
VALUES (silverIntegrationTimestamp, sessionId, providerId, providerName, jumpOffId, auditId)
    ON CONFLICT ON CONSTRAINT silverintegrationjumpoffmetric_auditid_unique
    DO NOTHING;
END;
$$ LANGUAGE plpgsql;