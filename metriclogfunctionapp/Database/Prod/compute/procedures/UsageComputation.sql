CREATE OR REPLACE PROCEDURE compute.UsageComputation(
    "startDate" timestamp without time zone,
    "endDate" timestamp without time zone,
    "tableName" character varying)
AS $$

DECLARE
    usagePrefix varchar := format('Usage_%s_', "tableName");
    usageComputationLogId int;
    tempTableCreationLogId int;
    loginsLogId int;
    usersLogId int;
    prescriptionLogId int;
    recordViewLogId int;
    odRegistrationsLogId int;
    odWithdrawalsLogId int;
    odUpdatesLogId int;
    odLookupsLogId int;
    appointmentsBookedLogId int;
    appointmentsCancelledLogId int;
    nomPharmacyLogId int;
    recordDCRLogId int;
    recordSCRLogId int;
    p5NewAppUsersLogId int;
    acceptedTermsAndConditionsLogId int;
    p9VerifiedNHSAppUsersLogId int;
    insertFromCountsLogId int;

    logResult bool;

BEGIN
    usageComputationLogId = audit.insertprocessduration(concat('Usage_', "tableName"));

    CREATE TEMP TABLE IF NOT EXISTS Counts (
                                               "Date" timestamp with time zone NOT NULL UNIQUE,
                                               "P5NewAppUsers" int,
                                               "AcceptedTermsAndConditions" int,
                                               "P9VerifiedNHSAppUsers" int,
                                               "Logins" int,
                                               "UsersLogin" int,
                                               "RecordViews" int,
                                               "UsersRecordViews" int,
                                               "Prescriptions" int,
                                               "UsersPrescriptions" int,
                                               "ODRegistrations" int,
                                               "UsersODRegistrations" int,
                                               "ODWithdrawals" int,
                                               "UsersODWithdrawals" int,
                                               "ODUpdates" int,
                                               "UsersODUpdates" int,
                                               "ODLookups" int,
                                               "UsersODLookups" int,
                                               "NomPharmacy" int,
                                               "UsersNomPharmacy" int,
                                               "AppointmentsBooked" int,
                                               "UsersAppointmentsBooked" int,
                                               "AppointmentsCancelled" int,
                                               "UsersAppointmentsCancelled" int,
                                               "RecordViewsDCR" int,
                                               "UsersRecordViewsDCR" int,
                                               "RecordViewsSCR" int,
                                               "UsersRecordViewsSCR" int);

    loginsLogId = audit.insertprocessduration(concat(usagePrefix, 'Cumulative Transactions'));
    IF "tableName" = 'DailyUsage' THEN
        INSERT INTO Counts ("Date", "Logins", "RecordViews", "Prescriptions", "ODRegistrations", "ODWithdrawals", "ODUpdates", "ODLookups", "NomPharmacy", "AppointmentsBooked", "AppointmentsCancelled", "RecordViewsDCR", "RecordViewsSCR")
        SELECT "startDate",
               SUM("Logins") ,
               SUM("RecordViews") ,
               SUM("Prescriptions") ,
               SUM("ODRegistrations") ,
               SUM("ODWithdrawals") ,
               SUM("ODUpdates") ,
               SUM("ODLookups") ,
               SUM("NomPharmacyUpdate") + SUM("NomPharmacyCreate") ,
               SUM("AppointmentsBooked"),
               SUM("AppointmentsCancelled"),
               SUM("RecordViewsDCR"),
               SUM("RecordViewsSCR")
        FROM "compute"."DailyUserTransactions"
        WHERE ("Date" >= "startDate")
          AND ("Date" < "endDate");
    ELSE
        INSERT INTO Counts ("Date", "P5NewAppUsers", "AcceptedTermsAndConditions", "P9VerifiedNHSAppUsers", "Logins", "RecordViews", "Prescriptions", "ODRegistrations", "ODWithdrawals", "ODUpdates", "ODLookups", "NomPharmacy", "AppointmentsBooked", "AppointmentsCancelled", "RecordViewsDCR", "RecordViewsSCR")
        SELECT "startDate",
               SUM("P5NewAppUsers"),
               SUM("AcceptedTermsAndConditions"),
               SUM("P9VerifiedNHSAppUsers"),
               SUM("Logins") ,
               SUM("RecordViews") ,
               SUM("Prescriptions") ,
               SUM("ODRegistrations") ,
               SUM("ODWithdrawals") ,
               SUM("ODUpdates") ,
               SUM("ODLookups") ,
               SUM("NomPharmacy") ,
               SUM("AppointmentsBooked"),
               SUM("AppointmentsCancelled"),
               SUM("RecordViewsDCR"),
               SUM("RecordViewsSCR")
        FROM "compute"."DailyUsage"
        WHERE ("Date" >= "startDate")
          AND ("Date" < "endDate");
    END IF;
    logResult = audit.updateprocessduration(loginsLogId);

    usersLogId = audit.insertprocessduration(concat(usagePrefix, 'UsersLogin'));
    INSERT INTO Counts ("Date", "UsersLogin")
    SELECT "startDate", count(DISTINCT "LoginId") as "UsersLogin"
    FROM "compute"."DailyUserTransactions" transactionTable
    WHERE (transactionTable."Date" >= "startDate")
      AND (transactionTable."Date" < "endDate")
      AND (transactionTable."Logins" > 0)
    ON CONFLICT("Date") DO UPDATE
        SET "UsersLogin" = Excluded."UsersLogin"
    WHERE Counts."UsersLogin" IS NULL;
    logResult = audit.updateprocessduration(usersLogId);

    prescriptionLogId = audit.insertprocessduration(concat(usagePrefix, 'UsersPrescriptions'));
    INSERT INTO Counts ("Date", "UsersPrescriptions")
    SELECT "startDate", count(DISTINCT "LoginId") as "UsersPrescriptions"
    FROM "compute"."DailyUserTransactions" transactionTable
    WHERE (transactionTable."Date" >= "startDate")
      AND (transactionTable."Date" < "endDate")
      AND (transactionTable."Prescriptions" > 0)
    ON CONFLICT("Date") DO UPDATE
        SET "UsersPrescriptions" = Excluded."UsersPrescriptions"
    WHERE Counts."UsersPrescriptions" IS NULL;
    logResult = audit.updateprocessduration(prescriptionLogId);

    recordViewLogId = audit.insertprocessduration(concat(usagePrefix, 'UsersRecordViews'));
    INSERT INTO Counts ("Date", "UsersRecordViews")
    SELECT "startDate", count(DISTINCT "LoginId") as "UsersRecordViews"
    FROM "compute"."DailyUserTransactions" transactionTable
    WHERE (transactionTable."Date" >= "startDate")
      AND (transactionTable."Date" < "endDate")
      AND (transactionTable."RecordViews" > 0)
    ON CONFLICT("Date") DO UPDATE
        SET "UsersRecordViews" = Excluded."UsersRecordViews"
    WHERE Counts."UsersRecordViews" IS NULL;
    logResult = audit.updateprocessduration(recordViewLogId);

    odRegistrationsLogId = audit.insertprocessduration(concat(usagePrefix, 'UsersODRegistrations'));
    INSERT INTO Counts ("Date", "UsersODRegistrations")
    SELECT "startDate", count(DISTINCT "LoginId") as "UsersODRegistrations"
    FROM "compute"."DailyUserTransactions" transactionTable
    WHERE (transactionTable."Date" >= "startDate")
      AND (transactionTable."Date" < "endDate")
      AND (transactionTable."ODRegistrations" > 0)
    ON CONFLICT("Date") DO UPDATE
        SET "UsersODRegistrations" = Excluded."UsersODRegistrations"
    WHERE Counts."UsersODRegistrations" IS NULL;
    logResult = audit.updateprocessduration(odRegistrationsLogId);

    odWithdrawalsLogId = audit.insertprocessduration(concat(usagePrefix, 'UsersODWithdrawals'));
    INSERT INTO Counts ("Date", "UsersODWithdrawals")
    SELECT "startDate", count(DISTINCT "LoginId") as "UsersODWithdrawals"
    FROM "compute"."DailyUserTransactions" transactionTable
    WHERE (transactionTable."Date" >= "startDate")
      AND (transactionTable."Date" < "endDate")
      AND (transactionTable."ODWithdrawals" > 0)
    ON CONFLICT("Date") DO UPDATE
        SET "UsersODWithdrawals" = Excluded."UsersODWithdrawals"
    WHERE Counts."UsersODWithdrawals" IS NULL;
    logResult = audit.updateprocessduration(odWithdrawalsLogId);

    odUpdatesLogId = audit.insertprocessduration(concat(usagePrefix, 'UsersODUpdates'));
    INSERT INTO Counts ("Date", "UsersODUpdates")
    SELECT "startDate", count(DISTINCT "LoginId") as "UsersODUpdates"
    FROM "compute"."DailyUserTransactions" transactionTable
    WHERE (transactionTable."Date" >= "startDate")
      AND (transactionTable."Date" < "endDate")
      AND (transactionTable."ODUpdates" > 0)
    ON CONFLICT("Date") DO UPDATE
        SET "UsersODUpdates" = Excluded."UsersODUpdates"
    WHERE Counts."UsersODUpdates" IS NULL;
    logResult = audit.updateprocessduration(odUpdatesLogId);

    odLookupsLogId = audit.insertprocessduration(concat(usagePrefix, 'UsersODLookups'));
    INSERT INTO Counts ("Date", "UsersODLookups")
    SELECT "startDate", count(DISTINCT "LoginId") as "UsersODLookups"
    FROM "compute"."DailyUserTransactions" transactionTable
    WHERE (transactionTable."Date" >= "startDate")
      AND (transactionTable."Date" < "endDate")
      AND (transactionTable."ODLookups" > 0)
    ON CONFLICT("Date") DO UPDATE
        SET "UsersODLookups" = Excluded."UsersODLookups"
    WHERE Counts."UsersODLookups" IS NULL;
    logResult = audit.updateprocessduration(odLookupsLogId);

    appointmentsBookedLogId = audit.insertprocessduration(concat(usagePrefix, 'UsersAppointmentsBooked'));
    INSERT INTO Counts ("Date", "UsersAppointmentsBooked")
    SELECT "startDate", count(DISTINCT "LoginId") as "UsersAppointmentsBooked"
    FROM "compute"."DailyUserTransactions" transactionTable
    WHERE (transactionTable."Date" >= "startDate")
      AND (transactionTable."Date" < "endDate")
      AND (transactionTable."AppointmentsBooked" > 0)
    ON CONFLICT("Date") DO UPDATE
        SET "UsersAppointmentsBooked" = Excluded."UsersAppointmentsBooked"
    WHERE Counts."UsersAppointmentsBooked" IS NULL;
    logResult = audit.updateprocessduration(appointmentsBookedLogId);

    appointmentsCancelledLogId = audit.insertprocessduration(concat(usagePrefix, 'UsersAppointmentsCancelled'));
    INSERT INTO Counts ("Date", "UsersAppointmentsCancelled")
    SELECT "startDate", count(DISTINCT "LoginId") as "UsersAppointmentsCancelled"
    FROM "compute"."DailyUserTransactions" transactionTable
    WHERE (transactionTable."Date" >= "startDate")
      AND (transactionTable."Date" < "endDate")
      AND (transactionTable."AppointmentsCancelled" > 0)
    ON CONFLICT("Date") DO UPDATE
        SET "UsersAppointmentsCancelled" = Excluded."UsersAppointmentsCancelled"
    WHERE Counts."UsersAppointmentsCancelled" IS NULL;
    logResult = audit.updateprocessduration(appointmentsCancelledLogId);

    nomPharmacyLogId = audit.insertprocessduration(concat(usagePrefix, 'UsersNomPharmacy'));
    INSERT INTO Counts ("Date", "UsersNomPharmacy")
    SELECT "startDate", count(DISTINCT "LoginId") as "UsersNomPharmacy"
    FROM "compute"."DailyUserTransactions" transactionTable
    WHERE (transactionTable."Date" >= "startDate")
      AND (transactionTable."Date" < "endDate")
      AND (transactionTable."NomPharmacyCreate" > 0 OR transactionTable."NomPharmacyUpdate" > 0 )
    ON CONFLICT("Date") DO UPDATE
        SET "UsersNomPharmacy" = Excluded."UsersNomPharmacy"
    WHERE Counts."UsersNomPharmacy" IS NULL;
    logResult = audit.updateprocessduration(nomPharmacyLogId);

    recordDCRLogId = audit.insertprocessduration(concat(usagePrefix, 'UsersRecordViewsDCR'));
    INSERT INTO Counts ("Date", "UsersRecordViewsDCR")
    SELECT "startDate", count(DISTINCT "LoginId") as "UsersRecordViewsDCR"
    FROM "compute"."DailyUserTransactions" transactionTable
    WHERE (transactionTable."Date" >= "startDate")
      AND (transactionTable."Date" < "endDate")
      AND (transactionTable."RecordViewsDCR" > 0 )
    ON CONFLICT("Date") DO UPDATE
        SET "UsersRecordViewsDCR" = Excluded."UsersRecordViewsDCR"
    WHERE Counts."UsersRecordViewsDCR" IS NULL;
    logResult = audit.updateprocessduration(recordDCRLogId);

    recordSCRLogId = audit.insertprocessduration(concat(usagePrefix, 'UsersRecordViewsSCR'));
    INSERT INTO Counts ("Date", "UsersRecordViewsSCR")
    SELECT "startDate", count(DISTINCT "LoginId") as "UsersRecordViewsSCR"
    FROM "compute"."DailyUserTransactions" transactionTable
    WHERE (transactionTable."Date" >= "startDate")
      AND (transactionTable."Date" < "endDate")
      AND (transactionTable."RecordViewsSCR" > 0 )
    ON CONFLICT("Date") DO UPDATE
        SET "UsersRecordViewsSCR" = Excluded."UsersRecordViewsSCR"
    WHERE Counts."UsersRecordViewsSCR" IS NULL;
    logResult = audit.updateprocessduration(recordSCRLogId);

    IF "tableName" = 'DailyUsage' THEN
        p5NewAppUsersLogId = audit.insertprocessduration(concat(usagePrefix, 'P5NewAppUsers'));
        INSERT INTO Counts ("Date", "P5NewAppUsers")
        SELECT "startDate", count(*) AS "P5NewAppUsers"
        FROM "compute"."FirstLogins" transactionTable
        WHERE (transactionTable."ConsentDate" >= "startDate")
          AND (transactionTable."ConsentDate" < "endDate")
          AND "ConsentProofLevel" = 'P5'
        ON CONFLICT("Date") DO UPDATE
            SET "P5NewAppUsers" = Excluded."P5NewAppUsers"
        WHERE Counts."P5NewAppUsers" IS NULL;
        logResult = audit.updateprocessduration(p5NewAppUsersLogId);

        acceptedTermsAndConditionsLogId = audit.insertprocessduration(concat(usagePrefix, 'AcceptedTermsAndConditions'));
        INSERT INTO Counts ("Date", "AcceptedTermsAndConditions")
        SELECT "startDate", count(*) AS "AcceptedTermsAndConditions"
        FROM "compute"."FirstLogins" transactionTable
        WHERE (transactionTable."ConsentDate" >= "startDate")
          AND (transactionTable."ConsentDate" < "endDate")
        ON CONFLICT("Date") DO UPDATE
            SET "AcceptedTermsAndConditions" = Excluded."AcceptedTermsAndConditions"
        WHERE Counts."AcceptedTermsAndConditions" IS NULL;
        logResult = audit.updateprocessduration(acceptedTermsAndConditionsLogId);

        p9VerifiedNHSAppUsersLogId = audit.insertprocessduration(concat(usagePrefix, 'P9VerifiedNHSAppUsers'));
        INSERT INTO Counts ("Date", "P9VerifiedNHSAppUsers")
        SELECT "startDate", count(*) AS "P9VerifiedNHSAppUsers"
        FROM "compute"."FirstLogins"
        WHERE (
                "FirstP5LoginDate" IS NOT NULL
                AND ("FirstP9LoginDate" >= "startDate")
                AND ("FirstP9LoginDate" < "endDate"))
           OR ("ConsentProofLevel" = 'P9'
            AND ("ConsentDate" >= "startDate")
            AND ("ConsentDate" < "endDate"))
        ON CONFLICT("Date") DO UPDATE
            SET "P9VerifiedNHSAppUsers" = Excluded."P9VerifiedNHSAppUsers"
        WHERE Counts."P9VerifiedNHSAppUsers" IS NULL;
        logResult = audit.updateprocessduration(p9VerifiedNHSAppUsersLogId);
    END IF;

    insertFromCountsLogId = audit.insertprocessduration(concat(usagePrefix, 'InsertFromTempTable'));
    EXECUTE format(
            'INSERT INTO "compute".%I (
                "Date",
                "P5NewAppUsers",
                "AcceptedTermsAndConditions",
                "P9VerifiedNHSAppUsers",
                "Logins",
                "UsersLogin",
                "RecordViews",
                "UsersRecordViews",
                "Prescriptions",
                "UsersPrescriptions",
                "ODRegistrations",
                "UsersODRegistrations",
                "ODWithdrawals",
                "UsersODWithdrawals",
                "ODUpdates",
                "UsersODUpdates",
                "ODLookups",
                "UsersODLookups",
                "NomPharmacy",
                "UsersNomPharmacy",
                "AppointmentsBooked",
                "UsersAppointmentsBooked",
                "AppointmentsCancelled",
                "UsersAppointmentsCancelled",
                "RecordViewsDCR",
                "UsersRecordViewsDCR",
                "RecordViewsSCR",
                "UsersRecordViewsSCR")
            SELECT "Date",
                "P5NewAppUsers",
                "AcceptedTermsAndConditions",
                "P9VerifiedNHSAppUsers",
                "Logins",
                "UsersLogin",
                "RecordViews",
                "UsersRecordViews",
                "Prescriptions",
                "UsersPrescriptions",
                "ODRegistrations",
                "UsersODRegistrations",
                "ODWithdrawals",
                "UsersODWithdrawals",
                "ODUpdates",
                "UsersODUpdates",
                "ODLookups",
                "UsersODLookups",
                "NomPharmacy",
                "UsersNomPharmacy",
                "AppointmentsBooked",
                "UsersAppointmentsBooked",
                "AppointmentsCancelled",
                "UsersAppointmentsCancelled",
                "RecordViewsDCR",
                "UsersRecordViewsDCR",
                "RecordViewsSCR",
                "UsersRecordViewsSCR" FROM Counts;', "tableName");
    logResult = audit.updateprocessduration(insertFromCountsLogId);

    logResult = audit.updateprocessduration(usageComputationLogId);
END;
$$ LANGUAGE plpgsql;