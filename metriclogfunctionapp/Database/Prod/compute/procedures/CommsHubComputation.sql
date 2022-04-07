CREATE OR REPLACE PROCEDURE compute.CommsHubComputation(
    "startdate" timestamp with time zone,
    "enddate" timestamp with time zone)
AS $$
BEGIN

    -- On CONFLICT ON Constraint is done so that ReadEvents aren't recorded for Messages Sent which do not exist
    -- in the CommsHubMessagesSent table
    INSERT INTO compute."CommsHub" ("MessageId","LoginId","SendDate","MessageSendTimestamp","Type","Status","Supplier","CampaignRef","RequestId")
    SELECT
        chms."MessageId"::text
         ,chms."LoginId"::text
         ,date(chms."ProcessedTimestamp") AS "SendDate"
         ,chms."ProcessedTimestamp" AS "MessageSendTimestamp"
         ,chms."Type"
         ,chms."Status"
         ,COALESCE(chs."Supplier",'Not Mapped') as "Supplier"
         ,COALESCE(chr."CampaignRef",'None') as "CampaignRef"
         ,chms."RequestId"
    FROM events."CommsHubMessagesSent"  chms
             LEFT JOIN events."CommsHubRequests" chr
                       ON chms."RequestId"::text = chr."RequestId"::text
             LEFT JOIN "events"."CommsHubSuppliers" chs
                       ON chs."SupplierId" = chr."SupplierId"
    WHERE (chms."Timestamp" >= startDate) AND (chms."Timestamp" < endDate)
    ON CONFLICT ("MessageId") DO UPDATE
        SET "RequestId" =
                ( CASE
                      WHEN ( "CommsHub"."RequestId" IS NULL)
                          THEN Excluded."RequestId"
                      ELSE "CommsHub"."RequestId"
                    END),
            "CampaignRef" =
                ( CASE
                      WHEN ( "CommsHub"."CampaignRef" IS NULL)
                          THEN Excluded."CampaignRef"
                      ELSE "CommsHub"."CampaignRef"
                    END),
            "LoginId" =
                ( CASE
                      WHEN ( "CommsHub"."LoginId" IS NULL)
                          THEN Excluded."LoginId"
                      ELSE "CommsHub"."LoginId"
                    END),
            "SendDate" =
                ( CASE
                      WHEN ( "CommsHub"."SendDate" IS NULL)
                          THEN Excluded."SendDate"
                      ELSE "CommsHub"."SendDate"
                    END),
            "MessageSendTimestamp" =
                ( CASE
                      WHEN ( "CommsHub"."MessageSendTimestamp" IS NULL)
                          THEN Excluded."MessageSendTimestamp"
                      ELSE "CommsHub"."MessageSendTimestamp"
                    END),
            "Type" =
                ( CASE
                      WHEN ("CommsHub"."Type" IS NULL)
                          THEN Excluded."Type"
                      ELSE "CommsHub"."Type"
                    END),
            "Status" =
                ( CASE
                      WHEN ( "CommsHub"."Status" IS NULL)
                          THEN Excluded."Status"
                      ELSE "CommsHub"."Status"
                    END),
            "Supplier" =
                ( CASE
                      WHEN ( "CommsHub"."Supplier" IS NULL OR "CommsHub"."Supplier" = 'Not Mapped')
                          THEN COALESCE(Excluded."Supplier",'Not Mapped')
                      ELSE COALESCE("CommsHub"."Supplier",'Not Mapped')
                    END);

    INSERT INTO compute."CommsHub" ("MessageId","ReadEvents","MessageReadTimestamp")
    SELECT
        "MessageId",
        count(DISTINCT "Timestamp") as "ReadEvents",
        "Timestamp" as "MessageReadTimestamp"
    FROM events."CommsHubMessagesRead"
    WHERE ("Timestamp" >= startDate) AND ("Timestamp" < endDate)
    GROUP BY
        "MessageId"
           ,"Timestamp"
    ON CONFLICT("MessageId") DO UPDATE
        SET
            "MessageReadTimestamp" =
                ( CASE
                      WHEN ( "CommsHub"."MessageReadTimestamp" IS NULL)
                          THEN Excluded."MessageReadTimestamp"
                      ELSE "CommsHub"."MessageReadTimestamp"
                    END),
            "ReadEvents" =
                ( CASE
                      WHEN ( "CommsHub"."ReadEvents" IS NULL)
                          THEN Excluded."ReadEvents"
                      ELSE "CommsHub"."ReadEvents"
                    END);

    INSERT INTO compute."CommsHub" ("MessageId","LoggedIn","UserFirstLoginTimestamp")
    SELECT
        "MessageId",
        1 as "LoggedIn",
        "UserFirstLoginTimestamp"
    FROM compute."FirstLoginMessages"
    WHERE ("UserFirstLoginTimestamp" >= startDate) AND ("UserFirstLoginTimestamp" < endDate)
    ON CONFLICT("MessageId") DO UPDATE
        SET
            "UserFirstLoginTimestamp" =
                ( CASE
                      WHEN ( "CommsHub"."UserFirstLoginTimestamp" IS NULL)
                          THEN Excluded."UserFirstLoginTimestamp"
                      ELSE "CommsHub"."UserFirstLoginTimestamp"
                    END),
            "LoggedIn" =
                ( CASE
                      WHEN ( "CommsHub"."LoggedIn" IS NULL)
                          THEN Excluded."LoggedIn"
                      ELSE "CommsHub"."LoggedIn"
                    END);

	INSERT INTO compute."CommsHubPivot" ("LoginId","RequestId","SendDate","MessageSendTimestamp","InAppStatus","Supplier","CampaignRef","MessageReadTimestamp","InAppUserFirstLoginTimestamp")
	SELECT
        "LoginId",
        "RequestId",
        "SendDate",
        "MessageSendTimestamp",
        "Status",
        "Supplier",
        "CampaignRef",
        "MessageReadTimestamp",
        "UserFirstLoginTimestamp"
	FROM
        compute."CommsHub"
	WHERE "Type" = 'In-App'
	AND "SendDate" >= '2021-01-01'
	AND
	(
		(("MessageSendTimestamp" >= startDate) AND ("MessageSendTimestamp" < endDate))
		OR
		(("MessageReadTimestamp" >= startDate) AND ("MessageReadTimestamp" < endDate))
		OR
		(("UserFirstLoginTimestamp" >= startDate) AND ("UserFirstLoginTimestamp" < endDate))
	)
 	ON CONFLICT ("LoginId","RequestId") DO UPDATE
	    SET
			"SendDate" =
				( CASE
					  WHEN ( "CommsHubPivot"."SendDate" IS NULL)
						  THEN Excluded."SendDate"
					  ELSE "CommsHubPivot"."SendDate"
					END),
			"MessageSendTimestamp" =
				( CASE
					  WHEN ( "CommsHubPivot"."MessageSendTimestamp" IS NULL)
						  THEN Excluded."MessageSendTimestamp"
					  ELSE "CommsHubPivot"."MessageSendTimestamp"
					END),
			"InAppStatus" =
                ( CASE
                      WHEN ( "CommsHubPivot"."InAppStatus" IS NULL)
                          THEN Excluded."InAppStatus"
                      ELSE "CommsHubPivot"."InAppStatus"
                    END),
            "Supplier" =
                ( CASE
                      WHEN ( "CommsHubPivot"."Supplier" IS NULL OR "CommsHubPivot"."Supplier" = 'Not Mapped')
                          THEN COALESCE(Excluded."Supplier",' Not Mapped')
                      ELSE COALESCE("CommsHubPivot"."Supplier", 'Not Mapped')
                    END),
            "CampaignRef" =
                ( CASE
                      WHEN ( "CommsHubPivot"."CampaignRef" IS NULL)
                          THEN Excluded."CampaignRef"
                      ELSE "CommsHubPivot"."CampaignRef"
                    END),
            "MessageReadTimestamp" =
                ( CASE
                      WHEN ( "CommsHubPivot"."MessageReadTimestamp" IS NULL)
                          THEN Excluded."MessageReadTimestamp"
                      ELSE "CommsHubPivot"."MessageReadTimestamp"
                    END),
			"InAppUserFirstLoginTimestamp" =
                ( CASE
                      WHEN ( "CommsHubPivot"."InAppUserFirstLoginTimestamp" IS NULL)
                          THEN Excluded."InAppUserFirstLoginTimestamp"
                      ELSE "CommsHubPivot"."InAppUserFirstLoginTimestamp"
                    END);

	INSERT INTO compute."CommsHubPivot" ("LoginId","RequestId","PushStatus","PushUserFirstLoginTimestamp","SendDate","MessageSendTimestamp","Supplier","CampaignRef")
	SELECT
	"LoginId",
	"RequestId",
	"Status",
	"UserFirstLoginTimestamp",
	"SendDate",
	"MessageSendTimestamp",
	"Supplier",
	"CampaignRef"
	FROM
        compute."CommsHub"
	WHERE
	"Type" = 'Push'
	AND "SendDate" >= '2021-01-01'
	AND
	(
		(("MessageSendTimestamp" >= startDate) AND ("MessageSendTimestamp" < endDate))
		OR
		(("MessageReadTimestamp" >= startDate) AND ("MessageReadTimestamp" < endDate))
		OR
		(("UserFirstLoginTimestamp" >= startDate) AND ("UserFirstLoginTimestamp" < endDate))
	)
 	ON CONFLICT ("LoginId","RequestId") DO UPDATE
        SET "PushStatus" =
                ( CASE
                      WHEN ( "CommsHubPivot"."PushStatus" IS NULL)
                          THEN Excluded."PushStatus"
                      ELSE "CommsHubPivot"."PushStatus"
                    END),
			"PushUserFirstLoginTimestamp" =
                ( CASE
                      WHEN ( "CommsHubPivot"."PushUserFirstLoginTimestamp" IS NULL)
                          THEN Excluded."PushUserFirstLoginTimestamp"
                      ELSE "CommsHubPivot"."PushUserFirstLoginTimestamp"
                    END),
			"SendDate" =
                ( CASE
                      WHEN ( "CommsHubPivot"."SendDate" IS NULL)
                          THEN Excluded."SendDate"
                      ELSE "CommsHubPivot"."SendDate"
                    END),
			"MessageSendTimestamp" =
                ( CASE
                      WHEN ( "CommsHubPivot"."MessageSendTimestamp" IS NULL)
                          THEN Excluded."MessageSendTimestamp"
                      ELSE "CommsHubPivot"."MessageSendTimestamp"
                    END),
			"Supplier" =
                ( CASE
                      WHEN ( "CommsHubPivot"."Supplier" IS NULL OR "CommsHubPivot"."Supplier" = 'Not Mapped')
                          THEN COALESCE(Excluded."Supplier",' Not Mapped')
                      ELSE COALESCE("CommsHubPivot"."Supplier", 'Not Mapped')
                    END),
			"CampaignRef" =
                ( CASE
                      WHEN ( "CommsHubPivot"."CampaignRef" IS NULL)
                          THEN Excluded."CampaignRef"
                      ELSE "CommsHubPivot"."CampaignRef"
                    END);

END;
$$ LANGUAGE plpgsql;