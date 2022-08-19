CREATE OR REPLACE PROCEDURE compute.IntegratedPartnersComputation("startDate" timestamp with time zone, "endDate" timestamp with time zone)
AS $$

BEGIN

    CREATE TEMP TABLE JumpOffMetrics (
                                         "Timestamp" timestamp with time zone NOT NULL,
                                         "ProviderName" text,
                                         "JumpOffId" character varying,
                                         "SessionId" character varying(36));

    CREATE TEMP TABLE JumpOffEvents (
                                        "Date" date,
                                        "OdsCode" character varying(6),
                                        "Provider" text,
                                        "JumpOff" character varying);

    CREATE INDEX Date_Provider_JumpOff_OdsCode_idx ON JumpOffEvents ("Date", "OdsCode", "Provider", "JumpOff");

    INSERT INTO JumpOffMetrics ("Timestamp", "ProviderName", "JumpOffId", "SessionId")
    SELECT  "Timestamp"
         ,"ProviderName"
         ,"JumpOffId"
         ,"SessionId"
    FROM events."SilverIntegrationJumpOffMetric"
    WHERE "Timestamp" >= "startDate"
      AND "Timestamp" < "endDate"
      AND "ProviderName" not like ('%+%');

    INSERT INTO JumpOffMetrics ("Timestamp", "ProviderName", "JumpOffId", "SessionId")
    SELECT  "Timestamp"
         ,"ProviderName"
         ,"JumpOffId"
         ,"SessionId"
    FROM events."GoldIntegrationJumpOffMetric"
    WHERE "Timestamp" >= "startDate"
      AND "Timestamp" < "endDate";

    INSERT INTO JumpOffEvents ("Date","OdsCode","Provider","JumpOff")
    SELECT  jumpOffs."Timestamp"        AS "Date"
         ,logins."OdsCode"
         ,jumpOffs."ProviderName"    AS "Provider"
         ,jumpOffs."JumpOffId"       AS "JumpOff"
    FROM JumpOffMetrics jumpOffs
             LEFT JOIN events."LoginMetric" logins
                       ON jumpOffs."SessionId" = logins."SessionId"
    WHERE NOT EXISTS (
            SELECT  "OdsCode"
            FROM compute."TestOdsCodes"
            WHERE "OdsCode" = logins."OdsCode");

    INSERT INTO compute."IntegratedPartners" ("Date", "OdsCode", "Provider", "JumpOff", "Clicks")
    SELECT  "Date"
         ,"OdsCode"
         ,"Provider"
         ,"JumpOff"
         ,Count(*) AS "Clicks"
    FROM JumpOffEvents
    GROUP BY  "Date"
           ,"OdsCode"
           ,"Provider"
           ,"JumpOff"; END; $$ LANGUAGE plpgsql;
