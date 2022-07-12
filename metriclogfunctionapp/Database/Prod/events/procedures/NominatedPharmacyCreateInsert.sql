CREATE OR REPLACE PROCEDURE events.NominatedPharmacyCreateMetricInsert(
    createTimestamp timestamp with time zone,
    sessionId character varying(36),
    auditId character varying (36)
)
AS $$

BEGIN

    INSERT INTO "events"."NominatedPharmacyCreateMetric" (
        "Timestamp",
        "SessionId",
        "AuditId"
    )
    VALUES (
        createTimestamp,
        sessionId,
        auditId
    )
    ON CONFLICT ON CONSTRAINT nominatedpharmacycreatemetric_auditid_unique
        DO NOTHING;

END;

$$ LANGUAGE plpgsql;