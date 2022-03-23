CREATE OR REPLACE PROCEDURE compute.CommsHubSummary(
    "startDate" timestamp without time zone,
    "endDate" timestamp without time zone)
AS $$
DECLARE
    usagePrefix varchar := 'CommsHubSummary_';
    commsHubSummaryLogId int;
    newAppUsersLogId int;
    updateAppUsersLogId int;
    updateAppUsersReadLogId int;

    logResult bool;
BEGIN

    commsHubSummaryLogId = audit.insertprocessduration(concat(usagePrefix, 'Computation'));
    newAppUsersLogId = audit.insertprocessduration(concat(usagePrefix, 'Add_New_App_Users'));
    --Inserts new rows of aggregated AppUsers if no conflict, if there is a conflict the numbers of app users is updated (increased).
    INSERT INTO compute."CommsHubSummary" ("SendDate","Supplier","CampaignRef","Type","Status","LoggedIn","AppUsers","ReadBy")
    SELECT
        "SendDate"
         ,"Supplier"
         ,"CampaignRef"
         ,"Type"
         ,"Status"
         ,"LoggedIn"
         ,"OldAppUsers" + "AppUsers" AS "AppUsers"
         ,"ReadBy"
    FROM
        (
            SELECT
                ch."SendDate"
                 ,ch."Supplier"
                 ,ch."CampaignRef"
                 ,ch."Type"
                 ,ch."Status"
                 --if a user logs in and message arrives during session the login will not appear as a UserFirstLoginTimestamp. Here the MessageReadTimestamp is used instead.
                 ,CASE WHEN ch."UserFirstLoginTimestamp" IS NOT NULL AND (("UserFirstLoginTimestamp" >= "startDate") AND ("UserFirstLoginTimestamp" < "endDate")) THEN 'Yes'
                       WHEN ch."MessageReadTimestamp" IS NOT NULL AND (("MessageReadTimestamp" >= "startDate") AND ("MessageReadTimestamp" < "endDate")) THEN 'Yes'
                       ELSE 'No' END AS "LoggedIn"
                 ,count(ch."LoginId") AS "AppUsers"
                 --,ch."LoginId" AS "AppUsers"
                 ,COALESCE(chs."AppUsers",0) AS "OldAppUsers"
                 ,COALESCE(chs."ReadBy",0) AS "ReadBy"
            FROM --CommsHub ch change back
                 compute."CommsHub" ch
                     --joins to already existing CommsHubSummary using each attribute of the composite primary key
                     LEFT JOIN compute."CommsHubSummary" chs
                               ON ch."SendDate" = chs."SendDate"
                                   AND ch."Supplier" = chs."Supplier"
                                   AND ch."CampaignRef" = chs."CampaignRef"
                                   AND ch."Type" = chs."Type"
                                   AND ch."Status" = chs."Status"
                                   AND (CASE WHEN ch."UserFirstLoginTimestamp" IS NOT NULL AND (("UserFirstLoginTimestamp" >= "startDate") AND ("UserFirstLoginTimestamp" < "endDate")) THEN 'Yes'
                                             WHEN ch."MessageReadTimestamp" IS NOT NULL AND (("MessageReadTimestamp" >= "startDate") AND ("MessageReadTimestamp" < "endDate")) THEN 'Yes'
                                             ELSE 'No' END) = chs."LoggedIn"
            WHERE
                    ch."SendDate" >= '2021-01-01'
              AND
                (
                        (("MessageSendTimestamp" >= "startDate") AND ("MessageSendTimestamp" < "endDate"))
                        OR
                        --avoids double counting
                        (("UserFirstLoginTimestamp" >= "startDate") AND ("UserFirstLoginTimestamp" < "endDate") AND ("MessageReadTimestamp" IS NULL OR "MessageReadTimestamp" >="endDate"))
                        OR
                        --avoids double counting
                        (("MessageReadTimestamp" >= "startDate") AND ("MessageReadTimestamp" < "endDate") AND ("UserFirstLoginTimestamp" IS NULL OR "UserFirstLoginTimestamp" >="endDate"))
                        OR
                        (("UserFirstLoginTimestamp" >= "startDate") AND ("UserFirstLoginTimestamp" < "endDate") AND ("MessageReadTimestamp" >= "startDate") AND ("MessageReadTimestamp" < "endDate"))
                    )
            GROUP BY
                ch."SendDate"
                   ,ch."Supplier"
                   ,ch."CampaignRef"
                   ,ch."Type"
                   ,ch."Status"
                   ,CASE WHEN ch."UserFirstLoginTimestamp" IS NOT NULL AND (("UserFirstLoginTimestamp" >= "startDate") AND ("UserFirstLoginTimestamp" < "endDate")) THEN 'Yes'
                         WHEN ch."MessageReadTimestamp" IS NOT NULL AND (("MessageReadTimestamp" >= "startDate") AND ("MessageReadTimestamp" < "endDate")) THEN 'Yes'
                         ELSE 'No' END
                   ,COALESCE(chs."AppUsers",0)
                   ,COALESCE(chs."ReadBy",0)
        ) t1
    ON CONFLICT ("SendDate","Supplier","CampaignRef","Type","Status","LoggedIn") DO UPDATE
        SET "AppUsers" = Excluded."AppUsers";

    logResult = audit.updateprocessduration(newAppUsersLogId);

--Updates rows of aggregated users if users have logged in on a different day than the message was sent. If there is a conflict the number of app users who have not logged in is updated(decreased)
    updateAppUsersLogId = audit.insertprocessduration(concat(usagePrefix, 'Update_New_App_Users'));
    INSERT INTO compute."CommsHubSummary" ("SendDate","Supplier","CampaignRef","Type","Status","LoggedIn","AppUsers")

    SELECT
        "SendDate"
         ,"Supplier"
         ,"CampaignRef"
         ,"Type"
         ,"Status"
         ,"LoggedIn"
         ,"OldAppUsers" - "AppUsers" AS "AppUsers"
    FROM
        (
            SELECT
                ch."SendDate"
                 ,ch."Supplier"
                 ,ch."CampaignRef"
                 ,ch."Type"
                 ,ch."Status"
                 ,chs."LoggedIn"
                 ,count(ch."LoginId") AS "AppUsers"
                 ,COALESCE(chs."AppUsers",0) AS "OldAppUsers"
            FROM compute."CommsHub" ch
                     --joins to already existing CommsHubSummary using each attribute of the composite primary key
                     JOIN compute."CommsHubSummary" chs
                          ON ch."SendDate" = chs."SendDate"
                              AND ch."Supplier" = chs."Supplier"
                              AND ch."CampaignRef" = chs."CampaignRef"
                              AND ch."Type" = chs."Type"
                              AND ch."Status" = chs."Status"
            WHERE
                    ch."SendDate" >= '2021-01-01'
              AND chs."LoggedIn" = 'No'
              AND
                (
                    --avoids double counting
                        (("UserFirstLoginTimestamp" >= "startDate") AND ("UserFirstLoginTimestamp" < "endDate") AND ("MessageReadTimestamp" IS NULL OR "MessageReadTimestamp" >="endDate"))
                        OR
                        --avoids double counting
                        (("MessageReadTimestamp" >= "startDate") AND ("MessageReadTimestamp" < "endDate") AND ("UserFirstLoginTimestamp" IS NULL OR "UserFirstLoginTimestamp" >="endDate"))
                        OR
                        (("UserFirstLoginTimestamp" >= "startDate") AND ("UserFirstLoginTimestamp" < "endDate") AND ("MessageReadTimestamp" >= "startDate") AND ("MessageReadTimestamp" < "endDate"))
                    )
              --only updates if login/timestamp is in a different compute run
              AND
                (("MessageSendTimestamp" < "startDate") OR ("MessageSendTimestamp" >= "endDate"))
            GROUP BY
                ch."SendDate"
                   ,ch."Supplier"
                   ,ch."CampaignRef"
                   ,ch."Type"
                   ,ch."Status"
                   ,chs."LoggedIn"
                   ,COALESCE(chs."AppUsers",0)
        ) t1
    ON CONFLICT ("SendDate","Supplier","CampaignRef","Type","Status","LoggedIn") DO UPDATE
        SET "AppUsers" = Excluded."AppUsers";
    logResult = audit.updateprocessduration(updateAppUsersLogId);

--Updates rows of aggregated users if users have read the message. If there is a conflict the number of messages read will update (increase)
    updateAppUsersReadLogId = audit.insertprocessduration(concat(usagePrefix, 'Update_New_App_Users_Read'));
    INSERT INTO compute."CommsHubSummary" ("SendDate","Supplier","CampaignRef","Type","Status","LoggedIn","ReadBy")

    SELECT
        "SendDate"
         ,"Supplier"
         ,"CampaignRef"
         ,"Type"
         ,"Status"
         ,"LoggedIn"
         ,"ReadBy" + "OldReadBy" AS "ReadBy"
    FROM
        (
            SELECT
                ch."SendDate"
                 ,ch."Supplier"
                 ,ch."CampaignRef"
                 ,ch."Type"
                 ,ch."Status"
                 ,CASE WHEN ch."UserFirstLoginTimestamp" IS NOT NULL THEN 'Yes' WHEN ch."MessageReadTimestamp" IS NOT NULL THEN 'Yes' ELSE 'No' END AS "LoggedIn"
                 ,count(ch."MessageReadTimestamp") AS "ReadBy"
                 ,COALESCE(chs."ReadBy",0) AS "OldReadBy"
            FROM compute."CommsHub" ch
                     --joins to already existing CommsHubSummary using each attribute of the composite primary key
                     LEFT JOIN compute."CommsHubSummary" chs
                               ON ch."SendDate" = chs."SendDate"
                                   AND ch."Supplier" = chs."Supplier"
                                   AND ch."CampaignRef" = chs."CampaignRef"
                                   AND ch."Type" = chs."Type"
                                   AND ch."Status" = chs."Status"
                                   AND (CASE WHEN ch."UserFirstLoginTimestamp" IS NOT NULL THEN 'Yes' WHEN ch."MessageReadTimestamp" IS NOT NULL THEN 'Yes' ELSE 'No' END) = chs."LoggedIn"
            WHERE
                    ch."SendDate" >= '2021-01-01'
              AND
                (("MessageReadTimestamp" >= "startDate") AND ("MessageReadTimestamp" < "endDate"))
            GROUP BY
                ch."SendDate"
                   ,ch."Supplier"
                   ,ch."CampaignRef"
                   ,ch."Type"
                   ,ch."Status"
                   ,CASE WHEN ch."UserFirstLoginTimestamp" IS NOT NULL THEN 'Yes' WHEN ch."MessageReadTimestamp" IS NOT NULL THEN 'Yes' ELSE 'No' END
                   ,COALESCE(chs."ReadBy",0)
        ) t1
    ON CONFLICT ("SendDate","Supplier","CampaignRef","Type","Status","LoggedIn") DO UPDATE
        SET "ReadBy" = Excluded."ReadBy";
    logResult = audit.updateprocessduration(updateAppUsersReadLogId);
    logResult = audit.updateprocessduration(commsHubSummaryLogId);

END;
$$ LANGUAGE plpgsql;