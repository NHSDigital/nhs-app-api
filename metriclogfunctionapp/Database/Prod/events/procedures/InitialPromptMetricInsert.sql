CREATE OR REPLACE PROCEDURE events.InitialPromptMetricInsert (
    loginId character varying(36),
    clickedTimestamp timestamp with time zone,
    optedIn character varying(3),
    auditId character varying(36)
)
AS $$

BEGIN

    INSERT INTO "events"."InitialPromptMetric" (
            "LoginId",
            "Timestamp",
            "OptedIn",
            "AuditId"
        )
    VALUES (
            loginId,
            clickedTimestamp,
            optedIn,
            auditId
        )
    ON CONFLICT ON CONSTRAINT InitialPromptMetric_auditid_unique
    DO NOTHING;

END;

$$ LANGUAGE plpgsql;