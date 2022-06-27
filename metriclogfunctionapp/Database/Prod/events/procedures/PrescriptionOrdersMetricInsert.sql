CREATE OR REPLACE PROCEDURE events.PrescriptionOrdersMetricInsert (
    prescriptionOrdersMetricTimestamp timestamp with time zone,
    sessionId character varying(36),
    auditId character varying(36))
AS $$

BEGIN

    INSERT INTO "events"."PrescriptionOrdersMetric" ("Timestamp", "SessionId", "AuditId")
    VALUES (prescriptionOrdersMetricTimestamp, sessionId, auditId)
    ON CONFLICT ON CONSTRAINT prescriptionordersmetric_auditid_unique
    DO NOTHING;

END;

$$ LANGUAGE plpgsql;