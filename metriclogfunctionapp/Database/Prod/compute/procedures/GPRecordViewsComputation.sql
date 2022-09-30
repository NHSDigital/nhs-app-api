-- PROCEDURE: compute.GPRecordViewsComputation(timestamp with time zone, timestamp with time zone)

CREATE OR REPLACE PROCEDURE compute.GPRecordViewsComputation(
    "startDate" timestamp with time zone,
    "endDate" timestamp with time zone)
    LANGUAGE 'plpgsql'
AS $BODY$
BEGIN
    -- ************************
    -- GET SECTION VIEWS
    -- ************************
    DROP TABLE IF EXISTS "RecSectionViews";
    CREATE TEMP TABLE "RecSectionViews" AS
        (
            SELECT mrv."Timestamp"::date "Date", mrv."SessionId", mrv."Supplier", mrv."Section"
                 ,mrv."IsActingOnBehalfOfAnother"::varchar
            FROM "events"."MedicalRecordSectionViewMetric" mrv
            WHERE mrv."Timestamp" >= "startDate"  AND mrv."Timestamp" < "endDate"
        );

    CREATE INDEX ON "RecSectionViews"("SessionId");

    -- ****************************************
    -- GET CLICKTHROUGHS TO HEALTH RECORDS PAGE
    -- ****************************************
    DROP TABLE IF EXISTS "RecViews";
    CREATE TEMP TABLE "RecViews" AS
    SELECT
        mr."Timestamp"::date "Date",
        "SessionId", MAX(rv."Supplier") "Supplier",
        lm."OdsCode", lm."LoginId",
        COUNT(mr.*) "Views",
        MIN(mr."HasSummaryRecordAccess"::varchar) "HasSummaryRecordAccess", MIN(mr."HasDetailedRecordAccess"::varchar) "HasDetailedRecordAccess" -- check the true/false
    FROM "events"."MedicalRecordViewMetric" mr
             LEFT JOIN LATERAL
        (
        SELECT lm."OdsCode", lm."LoginId" FROM events."LoginMetric" lm WHERE lm."SessionId"=mr."SessionId" LIMIT 1 -- Time: 1011.812s - Session duplications
        ) AS lm ON TRUE
             LEFT JOIN LATERAL
        (
        SELECT "Supplier", "IsActingOnBehalfOfAnother"::varchar FROM "RecSectionViews" rv
        WHERE rv."SessionId" = mr."SessionId" LIMIT 1
        ) AS rv ON TRUE
    WHERE mr."Timestamp" >= "startDate"-INTERVAL '2 HOURS' AND mr."Timestamp" < "endDate"
    GROUP BY "SessionId", mr."Timestamp"::date, lm."OdsCode", lm."LoginId";

    CREATE INDEX ON "RecViews"("SessionId");
    CREATE INDEX ON "RecViews"("Date","SessionId");
    CREATE INDEX ON "RecSectionViews"("Date","SessionId");
    CREATE INDEX ON "RecViews"("OdsCode");

    DROP TABLE IF EXISTS "RecViewsExt";
    CREATE TEMP TABLE "RecViewsExt" AS
    SELECT rv.*, CASE WHEN A."SessionId" IS NOT NULL THEN 'true' ELSE 'false' END AS "IsActingOnBehalfOfAnother"
    FROM "RecViews" rv
             LEFT JOIN LATERAL
        (SELECT "SessionId" FROM "RecSectionViews" rv2 WHERE rv2."IsActingOnBehalfOfAnother" = 'true' AND rv2."SessionId"=rv."SessionId" LIMIT 1) AS A ON TRUE;

    CREATE INDEX ON "RecViewsExt"("SessionId");
    CREATE INDEX ON "RecViewsExt"("Date","SessionId");
    CREATE INDEX ON "RecViewsExt"("IsActingOnBehalfOfAnother");
    CREATE INDEX ON "RecViewsExt"("OdsCode");

    -- **************************************
    -- BUILD UP SUPPLIER TO ODS CODE MAPPING
    -- **************************************
    DROP TABLE IF EXISTS "SupplierRef";
    CREATE TEMP TABLE "SupplierRef" AS
    WITH CTE AS
             (
                 SELECT "OdsCode", "Supplier", MAX("Date") "Date" FROM "RecViews" WHERE "Supplier" <> 'Disconnected' GROUP BY "Supplier", "OdsCode" HAVING "OdsCode" IS NOT NULL
             ),CTE2 AS
             (
                 SELECT "OdsCode", "Supplier", "Date", row_number() OVER (PARTITION BY "OdsCode" ORDER BY "Date" DESC) "Occurence" FROM CTE
             )
    SELECT "OdsCode", "Supplier" FROM CTE2 WHERE "Occurence" = 1;

    CREATE INDEX ON "SupplierRef"("OdsCode");

    -- ****************************************
    -- PIVOT SECTIONS MANUALLY
    -- ****************************************
    DROP TABLE IF EXISTS "RecSectionViewsPvt";
    CREATE TEMP TABLE "RecSectionViewsPvt" AS
    SELECT "Date", "SessionId",
           SUM(CASE WHEN "Section" = 'PROCEDURES' THEN 1 ELSE 0 END) AS "PROCEDURES",
           SUM(CASE WHEN "Section" = 'EXAM FINDINGS' THEN 1 ELSE 0 END) AS "EXAM FINDINGS",
           SUM(CASE WHEN "Section" = 'HEALTH CONDITIONS' THEN 1 ELSE 0 END) AS "HEALTH CONDITIONS",
           SUM(CASE WHEN "Section" = 'CONSULTATIONS AND EVENTS' THEN 1 ELSE 0 END) AS "CONSULTATIONS AND EVENTS",
           SUM(CASE WHEN "Section" = 'DIAGNOSIS' THEN 1 ELSE 0 END) AS "DIAGNOSIS",
           SUM(CASE WHEN "Section" = 'IMMUNISATIONS' THEN 1 ELSE 0 END) AS "IMMUNISATIONS",
           SUM(CASE WHEN "Section" = 'ALLERGIES AND ADVERSE REACTIONS' THEN 1 ELSE 0 END) AS "ALLERGIES AND ADVERSE REACTIONS",
           SUM(CASE WHEN "Section" = 'DOCUMENTS' THEN 1 ELSE 0 END) AS "DOCUMENTS",
           SUM(CASE WHEN "Section" = 'MEDICINES' THEN 1 ELSE 0 END) AS "MEDICINES",
           SUM(CASE WHEN "Section" = 'TEST RESULTS' THEN 1 ELSE 0 END) AS "TEST RESULTS"
    FROM "RecSectionViews"
    GROUP BY "Date", "SessionId";

    CREATE INDEX ON "RecSectionViewsPvt" ("Date","SessionId");

    -- ****************************************
    -- DAY VIEW: Compute table
    -- ****************************************
    ALTER TABLE "RecViewsExt" DROP COLUMN "Supplier"; -- remove ambiguous column

    INSERT INTO compute."GPRecordViews"
    WITH CTE AS
             (
                 SELECT * FROM "RecViewsExt"
                                   LEFT JOIN "RecSectionViewsPvt" USING ("Date","SessionId")
                                   LEFT JOIN "SupplierRef" sr USING("OdsCode")
             )
    SELECT "OdsCode", "Date",MIN("Supplier") "Supplier"
         ,SUM("Views") "HealthRecordViews"
         ,COUNT(DISTINCT "LoginId") "UniqueUsers"
         ,COUNT(CASE WHEN "HasSummaryRecordAccess"='true' THEN 1 END) "ViewsWithSummaryRecordAccess"
         ,COUNT(CASE WHEN "HasDetailedRecordAccess"='true' THEN 1 END) "ViewsWithDetailedRecordAccess"
         ,COUNT(CASE WHEN "IsActingOnBehalfOfAnother"='true' THEN 1 END) "IsActingOnBehalfOfAnother"
         ,SUM("ALLERGIES AND ADVERSE REACTIONS") "AllergiesAdverseReactionsSectionViewCount"
         ,SUM("CONSULTATIONS AND EVENTS") "ConsultationEventsSectionViewCount"
         ,SUM("DIAGNOSIS") "DiagnosisSectionViewCount"
         ,SUM("DOCUMENTS") "DocumentsSectionViewCount"
         ,SUM("EXAM FINDINGS") "ExamFindingsSectionViewCount"
         ,SUM("HEALTH CONDITIONS") "HealthConditionsSectionViewCount"
         ,SUM("IMMUNISATIONS") "ImmunisationsSectionViewCount"
         ,SUM("MEDICINES") "MedicinesSectionViewCount"
         ,SUM("PROCEDURES") "ProceduresSectionViewCount"
         ,SUM("TEST RESULTS") "TestResultsSectionViewCount"
    FROM CTE
    WHERE "OdsCode" NOT IN (SELECT "OdsCode" FROM "compute"."TestOdsCodes")
    GROUP BY "Date","OdsCode"
    ON CONFLICT ON CONSTRAINT gprecordviews_unique DO UPDATE
        SET "OdsCode" = Excluded."OdsCode",
            "Date" = Excluded."Date",
            "Supplier" = Excluded."Supplier",
            "HealthRecordViews" = Excluded."HealthRecordViews",
            "UniqueUsers" = Excluded."UniqueUsers",
            "ViewsWithSummaryRecordAccess" = Excluded."ViewsWithSummaryRecordAccess",
            "ViewsWithDetailedRecordAccess" = Excluded."ViewsWithDetailedRecordAccess",
            "AllergiesAdverseReactionsSectionViewCount" = Excluded."AllergiesAdverseReactionsSectionViewCount",
            "ConsultationEventsSectionViewCount" = Excluded."ConsultationEventsSectionViewCount",
            "DiagnosisSectionViewCount" = Excluded."DiagnosisSectionViewCount",
            "DocumentsSectionViewCount" = Excluded."DocumentsSectionViewCount",
            "ExamFindingsSectionViewCount" = Excluded."ExamFindingsSectionViewCount",
            "HealthConditionsSectionViewCount" = Excluded."HealthConditionsSectionViewCount",
            "ImmunisationsSectionViewCount" = Excluded."ImmunisationsSectionViewCount",
            "MedicinesSectionViewCount" = Excluded."MedicinesSectionViewCount",
            "ProceduresSectionViewCount" = Excluded."ProceduresSectionViewCount",
            "TestResultsSectionViewCount" = Excluded."TestResultsSectionViewCount";
END;
$BODY$;