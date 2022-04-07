CREATE OR REPLACE PROCEDURE compute.MonthlyMedicalRecordViewUsageComputation(
    "monthStartDate" timestamp without time zone,
    "monthEndDate" timestamp without time zone)
AS $$
BEGIN

    CREATE TEMP TABLE IF NOT EXISTS UserTransactions (
                                                         "LoginId" character varying NOT NULL,
                                                         "Date" timestamp with time zone NOT NULL,
                                                         "RecordViewsDCR" INT DEFAULT 0,
                                                         "RecordViewsSCR" INT DEFAULT 0);

    CREATE INDEX IF NOT EXISTS usertransactions_date_loginid_idx ON UserTransactions ("Date","LoginId");
    INSERT INTO UserTransactions (
        "LoginId",
        "Date",
        "RecordViewsDCR",
        "RecordViewsSCR")
    SELECT
        "LoginId",
        "Date",
        "RecordViewsDCR",
        "RecordViewsSCR"
    FROM "compute"."DailyUserTransactions" transactionTable
    WHERE (transactionTable."Date" >= "monthStartDate")
      AND (transactionTable."Date" < "monthEndDate");

    UPDATE compute."MonthlyUsage"
        SET "UsersRecordViewsDCR" = (SELECT count(DISTINCT "LoginId") as "UsersRecordViewsDCR"
                                     FROM UserTransactions transactionTable
                                     WHERE (transactionTable."Date" >= "monthStartDate")
                                       AND (transactionTable."Date" < "monthEndDate")
                                       AND (transactionTable."RecordViewsDCR" > 0))
    WHERE "Date" = "monthStartDate";

    UPDATE compute."MonthlyUsage"
    SET "UsersRecordViewsSCR" = (SELECT count(DISTINCT "LoginId") as "UsersRecordViewsSCR"
                                 FROM UserTransactions transactionTable
                                 WHERE (transactionTable."Date" >= "monthStartDate")
                                   AND (transactionTable."Date" < "monthEndDate")
                                   AND (transactionTable."RecordViewsSCR" > 0))
    WHERE "Date" = "monthStartDate";

END;
$$ LANGUAGE plpgsql;