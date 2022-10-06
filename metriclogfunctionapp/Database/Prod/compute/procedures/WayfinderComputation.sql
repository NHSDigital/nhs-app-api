CREATE OR REPLACE PROCEDURE compute.WayfinderComputation(
        "startDate" timestamp with time zone,
        "endDate" timestamp with time zone
    )
AS $$
BEGIN


    DROP TABLE IF EXISTS "SecondarySummaryCareViews";
    CREATE TEMP TABLE "SecondarySummaryCareViews" AS
        (
            SELECT "Date",
                   SUM(case WHEN "TotalReferrals" >= 1 THEN 1 ELSE 0 END) AS "SessionsWithReferrals",
                   SUM(case WHEN "TotalUpcomingAppointments" >= 1 THEN 1 ELSE 0 END) AS "SessionsWithAppts",
                   SUM(case WHEN "TotalReferrals" >= 1 THEN "PageViews" ELSE 0 END) AS "ViewsWithReferrals",
                   SUM(case WHEN "TotalUpcomingAppointments" >= 1 THEN "PageViews" ELSE 0 END) AS "ViewsWithAppts",
                   SUM(case WHEN "TotalUpcomingAppointments" = 0 AND "TotalReferrals" = 0 THEN 1 ELSE 0 END) AS "Neither",
                   SUM(case WHEN "TotalUpcomingAppointments" > 0 AND "TotalReferrals" > 0 THEN 1 ELSE 0 END) AS "Both",
                   SUM(case WHEN "TotalUpcomingAppointments" > 0 OR "TotalReferrals" > 0 THEN 1 ELSE 0 END) AS "Either",
                   SUM("TotalUpcomingAppointments") AS "TotalAppts",
                   SUM("TotalReferrals") AS "TotalReferrals",
                   MAX("MaxRefs") AS "MostReferrals",
                   MAX("MaxAppts") AS "MostAppts",
                   COUNT(distinct("SessionId")) AS "Sessions",
                   SUM("PageViews") AS "Views"
            FROM (
                     SELECT DATE("Timestamp") AS "Date",
                            "SessionId",
                            COUNT("AuditId") AS "PageViews",
                            SUM("TotalReferrals") AS "TotalReferrals",
                            SUM("TotalUpcomingAppointments") AS "TotalUpcomingAppointments",
                            MAX("TotalReferrals") AS "MaxRefs",
                            MAX("TotalUpcomingAppointments") AS "MaxAppts"
                     FROM events."SecondaryCareSummaryMetric"
                     WHERE "Timestamp" >= "startDate"
                       AND "Timestamp" < "endDate"
                     GROUP BY "Date", "SessionId") AS "Sessions"
            GROUP BY "Date"
        );


    INSERT INTO compute."Wayfinder" (
        "Date",
        "SessionsWithReferrals",
        "SessionsWithAppts",
        "ViewsWithReferrals",
        "ViewsWithAppts",
        "Neither",
        "Both",
        "Either",
        "TotalAppts",
        "TotalReferrals",
        "MostReferrals",
        "MostAppts",
        "Sessions",
        "Views")
    SELECT SSCV."Date",
           SSCV."SessionsWithReferrals",
           SSCV."SessionsWithAppts",
           SSCV."ViewsWithReferrals",
           SSCV."ViewsWithAppts",
           SSCV."Neither",
           SSCV."Both",
           SSCV."Either",
           SSCV."TotalAppts",
           SSCV."TotalReferrals",
           SSCV."MostReferrals",
           SSCV."MostAppts",
           SSCV."Sessions",
           SSCV."Views"
    FROM "SecondarySummaryCareViews" SSCV
    ON CONFLICT("Date") DO UPDATE
    SET "SessionsWithReferrals" = Excluded."SessionsWithReferrals",
        "SessionsWithAppts" = Excluded."SessionsWithAppts",
        "ViewsWithReferrals" = Excluded."ViewsWithReferrals",
        "ViewsWithAppts" = Excluded."ViewsWithAppts",
        "Neither" = Excluded."Neither",
        "Both" = Excluded."Both",
        "Either" = Excluded."Either",
        "TotalAppts" = Excluded."TotalAppts",
        "TotalReferrals" = Excluded."TotalReferrals",
        "MostAppts" = Excluded."MostAppts",
        "MostReferrals" = Excluded."MostReferrals",
        "Sessions" = Excluded."Sessions",
        "Views" = Excluded."Views";
END;
$$ LANGUAGE plpgsql;