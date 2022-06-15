CREATE OR REPLACE PROCEDURE events.AppointmentBookMetricInsert (
    appointmentBookMetricTimestamp timestamp with time zone,
    sessionId character varying(36),
    auditId character varying(36))
AS $$

BEGIN

    INSERT INTO "events"."AppointmentBookMetric" ("Timestamp", "SessionId", "AuditId")
    VALUES (appointmentBookMetricTimestamp, sessionId, auditId)
    ON CONFLICT ON CONSTRAINT appointmentbookmetric_auditid_unique
    DO NOTHING;

END;

$$ LANGUAGE plpgsql;