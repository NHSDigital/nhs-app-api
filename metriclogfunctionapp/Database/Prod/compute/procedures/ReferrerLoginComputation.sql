CREATE OR REPLACE PROCEDURE compute.ReferrerLoginComputation(
        "startDate" timestamp with time zone,
        "endDate" timestamp with time zone
    ) AS $$

DECLARE 
    thisSprocName varchar := 'ReferrerLoginComputation';
    usagePrefix varchar := format('ReferrerLogin_');
    referrerLoginLogId int;
    referrerLoginsWithConsentForPeriodInsertLogId int;
    referrerLoginsWithoutConsentForPeriodInsertLogId int;
    newUserMetricsForPeriodInsertLogId int;
    existingUserMetricsForPeriodInsertLogId int;
	referrerLoginInsertLogId int;
    logResult bool;

BEGIN 

    referrerLoginLogId = audit.insertprocessduration(thisSprocName);

	-- Create temp tables used to build up the counts
	CREATE TEMP TABLE IF NOT EXISTS ReferrerLoginsWithConsentForPeriod (
                                         "ReferrerId" text,
                                         "ReferrerTimestamp" timestamp with time zone NOT NULL,
                                         "ConsentTimestamp" timestamp with time zone NOT NULL);

	CREATE TEMP TABLE IF NOT EXISTS ReferrerLoginsWithoutConsentForPeriod (
                                         "ReferrerId" text,
                                         "ReferrerTimestamp" timestamp with time zone NOT NULL,
                                         "LoginId" character varying(36));

	CREATE TEMP TABLE IF NOT EXISTS NewUserMetricsForPeriod ("Date" date,
                                         "ReferrerId" text,
                                         "UserCount" integer DEFAULT 0);
 		
	CREATE TEMP TABLE IF NOT EXISTS ExistingUserMetricsForPeriod ("Date" date,
                                         "ReferrerId" text,
                                         "UserCount" integer DEFAULT 0);

	-- Gather the referrer logins for today for users that have a consent record
    referrerLoginsWithConsentForPeriodInsertLogId = audit.insertprocessduration(concat(usagePrefix, 'ReferrerLoginsWithConsentForPeriod_Insert'));
	INSERT INTO ReferrerLoginsWithConsentForPeriod ("ReferrerId", "ReferrerTimestamp", "ConsentTimestamp")
		SELECT wir."Referrer", MIN(wir."Timestamp"), MIN(cm."Timestamp") 
		FROM events."WebIntegrationReferrals" wir
		INNER JOIN events."LoginMetric" lm on wir."SessionId" = lm."SessionId"
		INNER JOIN events."ConsentMetric" cm on lm."LoginId" = cm."LoginId"
		WHERE (wir."Timestamp" >= "startDate")
		AND (wir."Timestamp" < "endDate")
		GROUP By wir."Referrer", lm."LoginId";
    logResult = audit.updateprocessduration(referrerLoginsWithConsentForPeriodInsertLogId);

	-- Gather the referrer logins for today for users that do not have a consent record
    referrerLoginsWithoutConsentForPeriodInsertLogId = audit.insertprocessduration(concat(usagePrefix, 'ReferrerLoginsWithoutConsentForPeriod_Insert'));
	INSERT INTO ReferrerLoginsWithoutConsentForPeriod ("ReferrerId", "ReferrerTimestamp", "LoginId")
		SELECT wir."Referrer", MIN(wir."Timestamp"), lm."LoginId"
		FROM events."WebIntegrationReferrals" wir
		INNER JOIN events."LoginMetric" lm on wir."SessionId" = lm."SessionId"
		LEFT OUTER JOIN events."ConsentMetric" cm on lm."LoginId" = cm."LoginId"
		WHERE (wir."Timestamp" >= "startDate")
		AND (wir."Timestamp" < "endDate")
		AND (cm."Timestamp" IS NULL)
		GROUP By wir."Referrer", lm."LoginId";
    logResult = audit.updateprocessduration(referrerLoginsWithoutConsentForPeriodInsertLogId);

	-- Build a table of all new users (both those with and without a consent record)
    newUserMetricsForPeriodInsertLogId = audit.insertprocessduration(concat(usagePrefix, 'NewUserMetricsForPeriod_Insert'));
	INSERT INTO NewUserMetricsForPeriod ("Date", "ReferrerId", "UserCount")
		SELECT MIN(rf."ReferrerTimestamp"), rf."ReferrerId",  count(*) as "UserCount"
		FROM ReferrerLoginsWithConsentForPeriod rf		
		WHERE (rf."ConsentTimestamp" >= "startDate")
		AND (rf."ConsentTimestamp" < "endDate")
		GROUP By rf."ReferrerId";

	INSERT INTO NewUserMetricsForPeriod ("Date", "ReferrerId", "UserCount")
		SELECT MIN(rf."ReferrerTimestamp"), rf."ReferrerId",  count(*) as "UserCount"
		FROM ReferrerLoginsWithoutConsentForPeriod rf		
		INNER JOIN compute."FirstLogins" fl on rf."LoginId" = fl."LoginId"
		WHERE (fl."FirstP5LoginDate" IS NOT NULL)
		AND (fl."FirstP9LoginDate" IS NOT NULL)
		AND	(fl."FirstP9LoginDate" >= "startDate")
		AND (fl."FirstP9LoginDate" < "endDate")
		GROUP By rf."ReferrerId";
    logResult = audit.updateprocessduration(newUserMetricsForPeriodInsertLogId);

	-- Build a table of all existing users (both those with and without a consent record)
    existingUserMetricsForPeriodInsertLogId = audit.insertprocessduration(concat(usagePrefix, 'ExistingUserMetricsForPeriod_Insert'));
	INSERT INTO ExistingUserMetricsForPeriod ("Date", "ReferrerId", "UserCount")
		SELECT MIN(rf."ReferrerTimestamp"), rf."ReferrerId",  count(*) as "UserCount"
		FROM ReferrerLoginsWithConsentForPeriod rf		
		WHERE (rf."ConsentTimestamp" < "startDate")
		OR (rf."ConsentTimestamp" >= "endDate")
		GROUP By rf."ReferrerId";
		
	INSERT INTO ExistingUserMetricsForPeriod ("Date", "ReferrerId", "UserCount")
		SELECT MIN(rf."ReferrerTimestamp"), rf."ReferrerId",  count(*) as "UserCount"
		FROM ReferrerLoginsWithoutConsentForPeriod rf		
		INNER JOIN compute."FirstLogins" fl on rf."LoginId" = fl."LoginId"
		WHERE (fl."FirstP5LoginDate" IS NOT NULL)
		AND (fl."FirstP9LoginDate" IS NOT NULL)
		AND	(fl."FirstP9LoginDate" < "startDate")
		OR (fl."FirstP9LoginDate" >= "endDate")
		GROUP By rf."ReferrerId";
    logResult = audit.updateprocessduration(existingUserMetricsForPeriodInsertLogId);
		
	-- Insert the result from the temp tables into the ReferrerLogin table
    referrerLoginInsertLogId = audit.insertprocessduration('ReferrerLogin_Insert');
	INSERT INTO "compute"."ReferrerLogin" ("Date", "ReferrerId", "NewUsers")
		SELECT nu."Date", nu."ReferrerId", SUM(nu."UserCount")
		FROM NewUserMetricsForPeriod nu
		GROUP BY nu."Date", nu."ReferrerId"
	 ON CONFLICT("Date","ReferrerId") DO NOTHING;

	INSERT INTO "compute"."ReferrerLogin" ("Date", "ReferrerId", "ExistingUsers")
		SELECT eu."Date", eu."ReferrerId", SUM(eu."UserCount")
		FROM ExistingUserMetricsForPeriod eu
		GROUP BY eu."Date", eu."ReferrerId"
	ON CONFLICT("Date","ReferrerId") DO UPDATE
		SET "ExistingUsers" = Excluded."ExistingUsers";
    logResult = audit.updateprocessduration(referrerLoginInsertLogId);
	
	DROP TABLE IF EXISTS ReferrerLoginsWithConsentForPeriod;
	DROP TABLE IF EXISTS ReferrerLoginsWithoutConsentForPeriod;
	DROP TABLE IF EXISTS NewUserMetricsForPeriod;
	DROP TABLE IF EXISTS ExistingUserMetricsForPeriod;

    logResult = audit.updateprocessduration(referrerLoginLogId);

END;

$$ LANGUAGE plpgsql;