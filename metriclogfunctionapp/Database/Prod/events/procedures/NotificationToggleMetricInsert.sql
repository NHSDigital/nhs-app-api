CREATE OR REPLACE PROCEDURE events.NotificationToggleMetricInsert (
    loginId character varying(36),
    notificationToggleMetricTimestamp timestamp with time zone,
    notificationToggle character varying (3),
    auditId character varying(36)
)
AS $$

BEGIN

    INSERT INTO "events"."NotificationToggleMetric" (
            "LoginId",
            "Timestamp",
            "NotificationToggle",
            "AuditId"
        )
    VALUES (
            loginId,
            notificationToggleMetricTimestamp,
            notificationToggle,
            auditId
        )
    ON CONFLICT ON CONSTRAINT notificationtogglemetric_auditid_unique
    DO NOTHING;

END;

$$ LANGUAGE plpgsql;