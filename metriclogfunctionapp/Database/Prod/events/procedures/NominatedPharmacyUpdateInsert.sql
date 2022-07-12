CREATE OR REPLACE PROCEDURE events.NominatedPharmacyUpdateMetricInsert(
    updateTimestamp timestamp with time zone,
    sessionId character varying(36),
    auditId character varying (36)
)
AS $$

BEGIN

    INSERT INTO "events"."NominatedPharmacyUpdateMetric" (
        "Timestamp",
        "SessionId",
        "AuditId"
    )
    VALUES (
        updateTimestamp,
        sessionId,
        auditId
    )
    ON CONFLICT ON CONSTRAINT nominatedpharmacyupdatemetric_auditid_unique
        DO NOTHING;

END;

$$ LANGUAGE plpgsql;