CREATE OR REPLACE PROCEDURE compute.FirstLoginMessagesComputation(startDate timestamp with time zone, endDate timestamp with time zone)
AS $$

BEGIN

	CREATE TEMP TABLE LoginMetricsForPeriod(
		"LoginId" character varying(36) NOT NULL,
		"Timestamp" timestamp with time zone NOT NULL,
		"OdsCode" character varying(6)
		);

	INSERT INTO LoginMetricsForPeriod("LoginId", "Timestamp", "OdsCode")
	SELECT
		 "LoginId"
		,"Timestamp"
		,"OdsCode"
	FROM events."LoginMetric"
	WHERE ("Timestamp" >=  startDate) 
	  AND ("Timestamp" < endDate);

	INSERT INTO compute."FirstLoginMessages" ("MessageId", "LoginId","MessageSendTimestamp","UserFirstLoginTimestamp")
	SELECT 
		chms."MessageId" AS "MessageId",
		chms."LoginId",
		chms."ProcessedTimestamp" AS "MessageSendTimestamp",
		min(lm."Timestamp") AS "UserFirstLoginTimestamp"
	FROM LoginMetricsForPeriod lm
	LEFT JOIN events."CommsHubMessagesSent" chms
		ON lm."LoginId"::text = chms."LoginId"::text
		AND lm."Timestamp" >= chms."ProcessedTimestamp"
	WHERE "MessageId" IS NOT NULL
	  AND NOT EXISTS ( SELECT * FROM "compute"."TestOdsCodes" testOdsCodes WHERE testOdsCodes."OdsCode" = lm."OdsCode")
	GROUP BY
		chms."MessageId",
		chms."LoginId",
		chms."ProcessedTimestamp"
	ON CONFLICT ("MessageId") DO UPDATE
		SET "UserFirstLoginTimestamp" = Excluded."UserFirstLoginTimestamp"
	WHERE Excluded."UserFirstLoginTimestamp" >= Excluded."MessageSendTimestamp" 
	  AND Excluded."UserFirstLoginTimestamp" <= "FirstLoginMessages"."UserFirstLoginTimestamp";

	DROP TABLE IF EXISTS LoginMetricsForPeriod;

END;
$$ LANGUAGE plpgsql;