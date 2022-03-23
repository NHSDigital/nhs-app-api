DROP PROCEDURE IF EXISTS compute.FirstLoginsComputation(timestamp with time zone,timestamp with time zone);

CREATE OR REPLACE PROCEDURE compute.FirstLoginsComputation(startDateTime timestamp with time zone, endDateTime timestamp with time zone)
AS $$

DECLARE
    thisSprocName varchar := 'FirstLogins';
    firstLoginsLogId int;
    firstLogins_P5LogId int;
    firstLogins_p9LogId int;
    firstLogins_Consent_Insert_LogId int;
    firstLogins_Consent_Update_LogId int;
    logResult bool;

BEGIN
    firstLoginsLogId = audit.insertprocessduration(thisSprocName);

    firstLogins_P5LogId = audit.insertprocessduration('FirstLogins_P5');
        INSERT INTO "compute"."FirstLogins" ("LoginId", "FirstP5LoginDate", "FirstP5LoginTimestamp")
            SELECT "LoginId", MIN("Timestamp") AS "FirstP5LoginDate", MIN("Timestamp") AS "FirstP5LoginTimestamp"
            FROM "events"."LoginMetric"
            WHERE ("ProofLevel" = 'P5')
              AND ("Timestamp" >= startDateTime)
              AND ("Timestamp" < endDateTime)
            GROUP BY "LoginId"
            ON CONFLICT("LoginId") DO UPDATE
                SET "FirstP5LoginDate" =
                ( CASE
                      WHEN ( "FirstLogins"."FirstP5LoginDate" IS NULL
                          OR Excluded."FirstP5LoginDate" < "FirstLogins"."FirstP5LoginDate")
                          THEN Excluded."FirstP5LoginDate"
                      ELSE "FirstLogins"."FirstP5LoginDate"
                    END),
                "FirstP5LoginTimestamp" =
                ( CASE
                      WHEN ( "FirstLogins"."FirstP5LoginTimestamp" IS NULL
                          OR Excluded."FirstP5LoginTimestamp" < "FirstLogins"."FirstP5LoginTimestamp")
                          THEN Excluded."FirstP5LoginTimestamp"
                      ELSE "FirstLogins"."FirstP5LoginTimestamp"
                    END );
    logResult = audit.updateprocessduration(firstLogins_P5LogId);

    firstLogins_p9LogId = audit.insertprocessduration('FirstLogins_p9');
        INSERT INTO "compute"."FirstLogins" ("LoginId", "FirstP9LoginDate", "FirstP9LoginTimestamp")
            SELECT "LoginId", MIN("Timestamp") AS "FirstP9LoginDate", MIN("Timestamp") AS "FirstP9LoginTimestamp"
            FROM "events"."LoginMetric"
            WHERE ("ProofLevel" = 'P9')
              AND ("Timestamp" >= startDateTime)
              AND ("Timestamp" < endDateTime)
            GROUP BY "LoginId"
            ON CONFLICT("LoginId") DO UPDATE
                SET "FirstP9LoginDate" =
                ( CASE
                      WHEN ( "FirstLogins"."FirstP9LoginDate" IS NULL
                          OR Excluded."FirstP9LoginDate" < "FirstLogins"."FirstP9LoginDate")
                          THEN Excluded."FirstP9LoginDate"
                      ELSE "FirstLogins"."FirstP9LoginDate"
                    END),
                "FirstP9LoginTimestamp" =
                ( CASE
                      WHEN ( "FirstLogins"."FirstP9LoginTimestamp" IS NULL
                          OR Excluded."FirstP9LoginTimestamp" < "FirstLogins"."FirstP9LoginTimestamp" )
                          THEN Excluded."FirstP9LoginTimestamp"
                      ELSE "FirstLogins"."FirstP9LoginTimestamp"
                    END );
    logResult = audit.updateprocessduration(firstLogins_p9LogId);

    firstLogins_Consent_Insert_LogId = audit.insertprocessduration('FirstLogins_Consent_Insert');
        INSERT INTO "compute"."FirstLogins" ("LoginId", "ConsentDate", "ConsentProofLevel", "FirstP5LoginDate", "FirstP9LoginDate",
                                         "FirstP5LoginTimestamp", "FirstP9LoginTimestamp", "ConsentTimestamp")
            SELECT  "LoginId"
                 ,MIN("Timestamp")                                                    AS "ConsentDate"
                 ,"ProofLevel"                                                        AS "ConsentProofLevel"
                 ,(Case WHEN "ProofLevel" = 'P9' THEN NULL ELSE MIN("Timestamp") END) AS "FirstP5LoginDate"
                 ,(Case WHEN "ProofLevel" = 'P5' THEN NULL ELSE MIN("Timestamp") END) AS "FirstP9LoginDate"
                 ,(Case WHEN "ProofLevel" = 'P9' THEN NULL ELSE MIN("Timestamp") END) AS "FirstP5LoginTimestamp"
                 ,(Case WHEN "ProofLevel" = 'P5' THEN NULL ELSE MIN("Timestamp") END) AS "FirstP9LoginTimestamp"
                 ,MIN("Timestamp")                                                    AS "ConsentTimestamp"
            FROM "events"."ConsentMetric"
            WHERE ("Timestamp" >= startDateTime)
              AND ("Timestamp" < endDateTime)
            GROUP BY  "LoginId"
                   ,"ConsentProofLevel"
            ON CONFLICT("LoginId") DO NOTHING;
    logResult = audit.updateprocessduration(firstLogins_Consent_Insert_LogId);

    firstLogins_Consent_Update_LogId = audit.insertprocessduration('FirstLogins_Consent_Update');
        UPDATE "compute"."FirstLogins"
        SET "ConsentDate" =
            ( CASE
                  WHEN ("FirstLogins"."ConsentDate" IS NULL
                      OR c."ConsentDate" < "FirstLogins"."ConsentDate")
                      THEN c."ConsentDate"
                  ELSE "FirstLogins"."ConsentDate"
                END),
        "ConsentTimestamp" =
            ( CASE
                  WHEN ("FirstLogins"."ConsentTimestamp" IS NULL
                      OR c."ConsentTimestamp" < "FirstLogins"."ConsentTimestamp")
                      THEN c."ConsentTimestamp"
                  ELSE "FirstLogins"."ConsentTimestamp"
                END),
        "ConsentProofLevel" =
            ( CASE
                  WHEN ("FirstLogins"."ConsentDate" IS NULL
                      OR c."ConsentDate" < "FirstLogins"."ConsentDate")
                      THEN c."ConsentProofLevel"
                  ELSE "FirstLogins"."ConsentProofLevel"
                END)
        FROM
            (
            SELECT  "LoginId"
                 ,MIN("Timestamp") AS "ConsentDate"
                 ,"ProofLevel"     AS "ConsentProofLevel"
                 ,MIN("Timestamp") AS "ConsentTimestamp"
            FROM "events"."ConsentMetric"
            WHERE ("Timestamp" >= startDateTime)
              AND ("Timestamp" < endDateTime)
            GROUP BY  "LoginId"
                   ,"ConsentProofLevel"
        ) AS c
        WHERE "FirstLogins"."LoginId" = c."LoginId";
    logResult = audit.updateprocessduration(firstLogins_Consent_Update_LogId);

    logResult = audit.updateprocessduration(firstLoginsLogId);
END;

$$ LANGUAGE plpgsql;