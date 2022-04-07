CREATE OR REPLACE PROCEDURE compute.FirstLoginsComputation(loginId character varying,startDateTime timestamp with time zone, endDateTime timestamp with time zone)
AS $$
BEGIN

    INSERT INTO "compute"."FirstLogins" ("LoginId", "FirstP5LoginDate", "FirstP5LoginTimestamp")
    SELECT "LoginId", MIN("Timestamp") AS "FirstP5LoginDate", MIN("Timestamp") AS "FirstP5LoginTimestamp"
    FROM "events"."LoginMetric"
    WHERE ("ProofLevel" = 'P5')
      AND ("LoginId" = loginId)
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

    INSERT INTO "compute"."FirstLogins" ("LoginId", "FirstP9LoginDate", "FirstP9LoginTimestamp")
    SELECT "LoginId", MIN("Timestamp") AS "FirstP9LoginDate", MIN("Timestamp") AS "FirstP9LoginTimestamp"
    FROM "events"."LoginMetric"
    WHERE ("ProofLevel" = 'P9')
      AND ("LoginId" = loginId)
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
      AND ("LoginId" = loginId)
    GROUP BY  "LoginId"
           ,"ConsentProofLevel"
    ON CONFLICT("LoginId") DO NOTHING;

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
              AND ("LoginId" = loginId)
            GROUP BY  "LoginId"
                   ,"ConsentProofLevel"
        ) AS c
    WHERE "FirstLogins"."LoginId" = c."LoginId";

    INSERT INTO "compute"."FirstLogins"
    ("LoginId","LatestOdsCode","LatestProofLevel","LatestLoginTimestamp")
    SELECT
        t1."LoginId"
         ,t1."OdsCode"
         ,t1."ProofLevel"
         ,"LatestLoginTimestamp"
    FROM "events"."LoginMetric" t1
             JOIN
         (
             SELECT
                 max("Timestamp") as "LatestLoginTimestamp"
                  ,"LoginId"
             FROM "events"."LoginMetric"
             WHERE
                     "Timestamp" >= startDateTime AND
                     "Timestamp" < endDateTime AND
                     "LoginId" = loginId
             GROUP BY 2
         ) t2
         ON t1."LoginId"=t2."LoginId"
             AND t1."Timestamp"=t2."LatestLoginTimestamp"
    WHERE
            "Timestamp" >= startDateTime and
            "Timestamp" < endDateTime
      AND t1."LoginId" = loginId
    ON CONFLICT ("LoginId") DO UPDATE
        SET
            "LatestOdsCode" =
                ( CASE
                      WHEN ( Excluded."LatestOdsCode" <> "FirstLogins"."LatestOdsCode" OR "FirstLogins"."LatestOdsCode" IS NULL)
                          THEN Excluded."LatestOdsCode"
                      ELSE "FirstLogins"."LatestOdsCode"
                    END),
            "LatestProofLevel" =
                ( CASE
                      WHEN ( Excluded."LatestProofLevel" <> "FirstLogins"."LatestProofLevel"  OR "FirstLogins"."LatestProofLevel" IS NULL)
                          THEN Excluded."LatestProofLevel"
                      ELSE "FirstLogins"."LatestProofLevel"
                    END),
            "LatestLoginTimestamp" =
                ( CASE
                      WHEN ( Excluded."LatestLoginTimestamp" > "FirstLogins"."LatestLoginTimestamp" OR "FirstLogins"."LatestLoginTimestamp" IS NULL )
                          THEN Excluded."LatestLoginTimestamp"
                      ELSE "FirstLogins"."LatestLoginTimestamp"
                    END),
            "SingleLoginFlag" =
                ( CASE
                      WHEN ( Excluded."LatestLoginTimestamp" = "FirstLogins"."FirstP5LoginTimestamp" AND Excluded."LatestLoginTimestamp" = "FirstLogins"."FirstP9LoginTimestamp")
                          OR ( Excluded."LatestLoginTimestamp" = "FirstLogins"."FirstP5LoginTimestamp" AND "FirstLogins"."FirstP9LoginTimestamp" IS NULL)
                          OR ( Excluded."LatestLoginTimestamp" = "FirstLogins"."FirstP9LoginTimestamp" AND "FirstLogins"."FirstP5LoginTimestamp" IS NULL)
                          THEN 'Y'
                      ELSE 'N'
                    END);
END;

$$ LANGUAGE plpgsql;