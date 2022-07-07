CREATE OR REPLACE PROCEDURE events.BiometricsToggleMetricInsert (
    sessionId character varying(36),
    biometricsToggleMetricTimestamp timestamp with time zone,
    biometricsToggle character varying (3),
    auditId character varying(36)
)
AS $$

BEGIN

    INSERT INTO "events"."BiometricsToggleMetric" (
            "SessionId",
            "Timestamp",
            "BiometricsToggle",
            "AuditId"
        )
    VALUES (
            sessionId,
            biometricsToggleMetricTimestamp,
            biometricsToggle,
            auditId
        )
    ON CONFLICT ON CONSTRAINT biometricstogglemetric_auditid_unique
    DO NOTHING;

END;

$$ LANGUAGE plpgsql;