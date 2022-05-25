DROP PROCEDURE IF EXISTS events.MedicalRecordViewMetricInsert;

CREATE PROCEDURE events.MedicalRecordViewMetricInsert (
    medicalRecordViewMetricTimestamp timestamp with time zone,
    sessionId character varying(36),
    hasSummaryRecordAccess boolean,
    hasDetailedRecordAccess boolean,
    auditId character varying(36))
    AS $$

BEGIN

INSERT INTO "events"."MedicalRecordViewMetric" ("Timestamp", "SessionId", "HasSummaryRecordAccess", "HasDetailedRecordAccess", "AuditId")
VALUES (medicalRecordViewMetricTimestamp, sessionId, hasSummaryRecordAccess, hasDetailedRecordAccess, auditId)
    ON CONFLICT ON CONSTRAINT medicalrecordviewmetric_auditid_unique
    DO NOTHING;

END;

$$ LANGUAGE plpgsql;