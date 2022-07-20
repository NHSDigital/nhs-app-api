CREATE OR REPLACE PROCEDURE events.MedicalRecordSectionViewMetricInsert (
    medicalRecordSectionViewMetricTimestamp timestamp with time zone,
    sessionId character varying(36),
    supplier character varying(36),
    isActingOnBehalfOfAnother boolean,
    section	text,
    auditId character varying(36))
AS $$

BEGIN
    INSERT INTO events."MedicalRecordSectionViewMetric" ("Timestamp", "SessionId", "Supplier", "IsActingOnBehalfOfAnother", "Section", "AuditId")
    VALUES (medicalRecordSectionViewMetricTimestamp, sessionId, supplier, isActingOnBehalfOfAnother, section, auditId)
        ON CONFLICT ON CONSTRAINT medicalrecordsectionviewmetric_auditid_unique
        DO NOTHING;
END;

$$ LANGUAGE plpgsql;