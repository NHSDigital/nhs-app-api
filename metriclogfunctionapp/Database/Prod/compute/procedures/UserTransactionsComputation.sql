CREATE OR REPLACE PROCEDURE compute.UserTransactionsComputation(
    "startDate" timestamp without time zone,
    "endDate" timestamp without time zone)
AS $$
DECLARE
    userTransactionLogId int;
    userCountBySessionLogId int;
    userInsertIntoTransactionsLogId int;

    logResult bool;
BEGIN
    userTransactionLogId = audit.insertprocessduration('DailyUserTransaction');
    CREATE TEMP TABLE IF NOT EXISTS Counts (
                                               "SessionId" character varying UNIQUE,
                                               "Logins" int,
                                               "RecordViews" int,
                                               "Prescriptions" int,
                                               "ODRegistrations" int,
                                               "ODWithdrawals"	int,
                                               "ODUpdates" int,
                                               "ODLookups" int,
                                               "NomPharmacyUpdate" int,
                                               "NomPharmacyCreate" int,
                                               "AppointmentsBooked" int,
                                               "AppointmentsCancelled" int,
                                               "RecordViewsDCR" int,
                                               "RecordViewsSCR" int);

    CREATE INDEX IF NOT EXISTS idx_SessionId ON Counts ((lower("SessionId")));

    userCountBySessionLogId = audit.insertprocessduration('UserCountBySession');
    CALL "compute"."countbysessionid"("startDate","endDate");
    logResult = audit.updateprocessduration(userCountBySessionLogId);

    userInsertIntoTransactionsLogId = audit.insertprocessduration('InsertIntoUserTransaction');
    INSERT INTO "compute"."DailyUserTransactions" (
        "Date",
        "LoginId",
        "Logins" ,
        "RecordViews" ,
        "Prescriptions" ,
        "ODRegistrations" ,
        "ODWithdrawals" ,
        "ODUpdates" ,
        "ODLookups" ,
        "NomPharmacyUpdate" ,
        "NomPharmacyCreate" ,
        "AppointmentsBooked",
        "AppointmentsCancelled",
        "RecordViewsDCR",
        "RecordViewsSCR")
    SELECT
        "startDate",
        COALESCE(loginTable."LoginId",'ffffffff-ffff-ffff-ffff-ffffffff'),
        COALESCE(SUM(Counts."Logins"),0) ,
        COALESCE(SUM(Counts."RecordViews"),0) ,
        COALESCE(SUM(Counts."Prescriptions"),0) ,
        COALESCE(SUM(Counts."ODRegistrations"),0) ,
        COALESCE(SUM(Counts."ODWithdrawals"),0) ,
        COALESCE(SUM(Counts."ODUpdates"),0) ,
        COALESCE(SUM(Counts."ODLookups"),0) ,
        COALESCE(SUM(Counts."NomPharmacyUpdate"),0) ,
        COALESCE(SUM(Counts."NomPharmacyCreate"),0),
        COALESCE(SUM(Counts."AppointmentsBooked"),0),
        COALESCE(SUM(Counts."AppointmentsCancelled"),0),
        COALESCE(SUM(Counts."RecordViewsDCR"),0),
        COALESCE(SUM(Counts."RecordViewsSCR"),0) FROM Counts
                                                         LEFT JOIN "events"."LoginMetric" loginTable
                                                                   ON Counts."SessionId" = loginTable."SessionId"
    WHERE NOT EXISTS ( SELECT * FROM "compute"."TestOdsCodes" testOdsCodes WHERE testOdsCodes."OdsCode" = loginTable."OdsCode")
    GROUP BY "LoginId";
    logResult = audit.updateprocessduration(userInsertIntoTransactionsLogId);

    DROP TABLE IF EXISTS Counts;
    logResult = audit.updateprocessduration(userTransactionLogId);
END;
$$ LANGUAGE plpgsql;