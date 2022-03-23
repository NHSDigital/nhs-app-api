CREATE OR REPLACE PROCEDURE compute.OdsUsageComputation(
    "startDate" timestamp without time zone,
    "endDate" timestamp without time zone)
AS $$
DECLARE
    odsUsageComputationLogId int;
    insertFromOdsTransactions int;
    odsLogId int;
    p5NewAppUsersLogId int;
    acceptedTermsAndConditionsLogId int;
    p9VerifiedNHSAppUsersLogId int;

    logResult bool;
BEGIN
    odsUsageComputationLogId = audit.insertprocessduration('odsUsageComputation');
    CREATE TEMP TABLE IF NOT EXISTS Counts (
                                               "Date" timestamp with time zone NOT NULL,
                                               "OdsCode" varchar (6) NOT NULL,
                                               "P5NewAppUsers" int,
                                               "AcceptedTermsAndConditions" int,
                                               "P9VerifiedNHSAppUsers" int,
                                               "Logins" int,
                                               "RecordViews" int,
                                               "Prescriptions" int,
                                               "ODRegistrations" int,
                                               "ODWithdrawals" int,
                                               "ODUpdates" int,
                                               "ODLookups" int,
                                               "NomPharmacy" int,
                                               "AppointmentsBooked" int,
                                               "AppointmentsCancelled" int,
                                               "RecordViewsDCR" int,
                                               "RecordViewsSCR" int);

    CREATE UNIQUE INDEX IF NOT EXISTS idx_date_odscode ON Counts("Date", "OdsCode");

    insertFromOdsTransactions = audit.insertprocessduration('InsertFromDailyOdsTransactions');
    INSERT INTO Counts ("Date", "OdsCode", "Logins", "RecordViews", "Prescriptions", "ODRegistrations", "ODWithdrawals", "ODUpdates", "ODLookups", "NomPharmacy", "AppointmentsBooked", "AppointmentsCancelled", "RecordViewsDCR" , "RecordViewsSCR")
    SELECT "startDate",
           "OdsCode",
           "Logins" ,
           "RecordViews" ,
           "Prescriptions" ,
           "ODRegistrations" ,
           "ODWithdrawals" ,
           "ODUpdates" ,
           "ODLookups" ,
           "NomPharmacyUpdate" + "NomPharmacyCreate" ,
           "AppointmentsBooked",
           "AppointmentsCancelled",
           "RecordViewsDCR",
           "RecordViewsSCR"
    FROM "compute"."DailyOdsTransactions" transactions
    WHERE ("Date" >= "startDate")
      AND ("Date" < "endDate");
    logResult = audit.updateprocessduration(insertFromOdsTransactions);

    CREATE TEMP TABLE IF NOT EXISTS LoginIdOdsCode (
                                                       "OdsCode" character varying(6) NOT NULL,
                                                       "LoginId" character varying NOT NULL);

    odsLogId = audit.insertprocessduration('OdsLogin');
    INSERT INTO LoginIdOdsCode ("OdsCode", "LoginId")
    SELECT DISTINCT LM."OdsCode", LM."LoginId"
    FROM events."LoginMetric" LM
    WHERE LM."Timestamp" >= "startDate"
      AND LM."Timestamp" < "endDate";
    logResult = audit.updateprocessduration(odsLogId);

    p5NewAppUsersLogId = audit.insertprocessduration('OdsP5NewAppUsers');
    INSERT INTO Counts ("Date", "OdsCode", "P5NewAppUsers")
    SELECT "startDate", LM."OdsCode", count(*) AS "P5NewAppUsers"
    FROM "compute"."FirstLogins" FLM
             INNER JOIN LoginIdOdsCode LM on FLM."LoginId" = LM."LoginId"
    WHERE (FLM."ConsentDate" >= "startDate")
      AND (FLM."ConsentDate" < "endDate")
      AND "ConsentProofLevel" = 'P5'
    GROUP BY LM."OdsCode"
    ON CONFLICT("Date","OdsCode") DO UPDATE
        SET "P5NewAppUsers" = Excluded."P5NewAppUsers"
    WHERE Counts."P5NewAppUsers" IS NULL;
    logResult = audit.updateprocessduration(p5NewAppUsersLogId);

    acceptedTermsAndConditionsLogId = audit.insertprocessduration('OdsAcceptedTermsAndConditions');
    INSERT INTO Counts ("Date", "OdsCode", "AcceptedTermsAndConditions")
    SELECT "startDate", LM."OdsCode",count(*) AS "AcceptedTermsAndConditions"
    FROM "compute"."FirstLogins" FLM
             INNER JOIN LoginIdOdsCode LM on FLM."LoginId" = LM."LoginId"
    WHERE (FLM."ConsentDate" >= "startDate")
      AND (FLM."ConsentDate" < "endDate")
    GROUP BY LM."OdsCode"
    ON CONFLICT("Date","OdsCode") DO UPDATE
        SET "AcceptedTermsAndConditions" = Excluded."AcceptedTermsAndConditions"
    WHERE Counts."AcceptedTermsAndConditions" IS NULL;
    logResult = audit.updateprocessduration(acceptedTermsAndConditionsLogId);

    p9VerifiedNHSAppUsersLogId = audit.insertprocessduration('OdsP9VerifiedNHSAppUsers');
    INSERT INTO Counts ("Date", "OdsCode", "P9VerifiedNHSAppUsers")
    SELECT "startDate", LM."OdsCode", count(*) AS "P9VerifiedNHSAppUsers"
    FROM "compute"."FirstLogins" FLM
             INNER JOIN LoginIdOdsCode LM on FLM."LoginId" = LM."LoginId"
    WHERE (
            "FirstP5LoginDate" IS NOT NULL
            AND ("FirstP9LoginDate" >= "startDate")
            AND ("FirstP9LoginDate" < "endDate"))
       OR ("ConsentProofLevel" = 'P9'
        AND ("ConsentDate" >= "startDate")
        AND ("ConsentDate" < "endDate"))
    GROUP BY LM."OdsCode"
    ON CONFLICT("Date","OdsCode") DO UPDATE
        SET "P9VerifiedNHSAppUsers" = Excluded."P9VerifiedNHSAppUsers"
    WHERE Counts."P9VerifiedNHSAppUsers" IS NULL;
    logResult = audit.updateprocessduration(p9VerifiedNHSAppUsersLogId);

    INSERT INTO "compute"."DailyOdsUsage" (
        "Date",
        "OdsCode",
        "P5NewAppUsers",
        "AcceptedTermsAndConditions",
        "P9VerifiedNHSAppUsers",
        "Logins",
        "RecordViews",
        "Prescriptions",
        "NomPharmacy",
        "AppointmentsBooked",
        "AppointmentsCancelled",
        "ODRegistrations",
        "ODWithdrawals",
        "ODUpdates",
        "ODLookups",
        "RecordViewsDCR" ,
        "RecordViewsSCR")
    SELECT "Date",
           "OdsCode",
           "P5NewAppUsers",
           "AcceptedTermsAndConditions",
           "P9VerifiedNHSAppUsers",
           "Logins",
           "RecordViews",
           "Prescriptions",
           "NomPharmacy",
           "AppointmentsBooked",
           "AppointmentsCancelled",
           "ODRegistrations",
           "ODWithdrawals",
           "ODUpdates",
           "ODLookups",
           "RecordViewsDCR" ,
           "RecordViewsSCR" FROM Counts
    ON CONFLICT DO NOTHING;

    logResult = audit.updateprocessduration(odsUsageComputationLogId);
END
$$ LANGUAGE plpgsql;