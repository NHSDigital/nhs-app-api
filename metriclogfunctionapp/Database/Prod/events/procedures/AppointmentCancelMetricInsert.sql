CREATE OR REPLACE PROCEDURE events.AppointmentCancelMetricInsert (
    appointmentCancelMetricTimestamp timestamp with time zone,
    sessionId character varying(36),
    auditId character varying(36))
AS $$

BEGIN

    INSERT INTO "events"."AppointmentCancelMetric" ("Timestamp", "SessionId", "AuditId")
    VALUES (appointmentCancelMetricTimestamp, sessionId, auditId)
    ON CONFLICT ON CONSTRAINT appointmentcancelmetric_auditid_unique
    DO NOTHING;

END;

$$ LANGUAGE plpgsql;