CREATE OR REPLACE PROCEDURE compute.WayfinderComputation(
        "startDate" timestamp with time zone,
        "endDate" timestamp with time zone
    ) 
AS $$ 
BEGIN
    INSERT INTO compute."Wayfinder"
    SELECT sc."Timestamp"::date "Date",
        COUNT(DISTINCT sc."SessionId") "TotalSessions",
        COUNT(sc."AuditId") "TotalViews",
        COUNT(DISTINCT lm."LoginId") "Users",
        SUM(sc."TotalReferrals") "TotalReferrals",
        SUM(sc."TotalUpcomingAppointments") "TotalUpcomingAppointments"
    FROM "events"."SecondaryCareSummaryMetric" sc
        LEFT JOIN events."LoginMetric" lm USING("SessionId")
    WHERE sc."Timestamp" >= "startDate"
        AND sc."Timestamp" < "endDate"
    GROUP BY sc."Timestamp"::date ON CONFLICT("Date") DO
    UPDATE
    SET "TotalSessions" = Excluded."TotalSessions",
        "TotalViews" = Excluded."TotalViews",
        "Users" = Excluded."Users",
        "TotalReferrals" = Excluded."TotalReferrals",
        "TotalUpcomingAppointments" = Excluded."TotalUpcomingAppointments";
END;
$$ LANGUAGE plpgsql;