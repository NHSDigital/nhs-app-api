DROP PROCEDURE IF EXISTS events.SecondaryCareSummaryMetricInsert;

CREATE PROCEDURE events.SecondaryCareSummaryMetricInsert (
    secondaryCareSummaryMetricTimestamp timestamp with time zone,
    sessionId character varying(36),
    totalReferrals int,
    totalUpcomingAppointments int,
    auditId character varying(36))
AS $$

BEGIN

    INSERT INTO "events"."SecondaryCareSummaryMetric" ("Timestamp", "SessionId", "TotalReferrals", "TotalUpcomingAppointments", "AuditId")
    VALUES (secondaryCareSummaryMetricTimestamp, sessionId, totalReferrals, totalUpcomingAppointments, auditId)
    ON CONFLICT ON CONSTRAINT secondarycaresummarymetric_auditid_unique
    DO NOTHING;

END;

$$ LANGUAGE plpgsql;