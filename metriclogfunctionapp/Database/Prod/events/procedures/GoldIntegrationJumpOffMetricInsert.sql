DROP PROCEDURE IF EXISTS events.GoldIntegrationJumpOffMetricInsert;

CREATE PROCEDURE events.GoldIntegrationJumpOffMetricInsert (
    goldIntegrationTimestamp timestamp with time zone,
    sessionId character varying(36),
    providerId character varying,
    providerName character varying(36),
    jumpOffId character varying(36),
    auditId character varying(36))
    AS $$

BEGIN
INSERT INTO "events"."GoldIntegrationJumpOffMetric" ("Timestamp", "SessionId", "ProviderId", "ProviderName", "JumpOffId", "AuditId")
VALUES (goldIntegrationTimestamp, sessionId, providerId, providerName, jumpOffId, auditId)
    ON CONFLICT ON CONSTRAINT goldIntegrationJumpOffMetric_auditid_unique
    DO NOTHING;
END;
$$ LANGUAGE plpgsql;