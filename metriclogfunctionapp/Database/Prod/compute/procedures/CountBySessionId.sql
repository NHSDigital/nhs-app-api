CREATE OR REPLACE PROCEDURE compute.countbysessionid(
    "startDate" timestamp without time zone,
    "endDate" timestamp without time zone)
AS $$

DECLARE
    usagePrefix varchar := format('DailyTransaction_');
    loginTransaction int;
    prescriptionTransaction int;
    recordTransaction int;
    oDRegistrationTransaction int;
    oDWithdrawalTransaction int;
    oDUpdateTransaction int;
    oDLookUpTransaction int;
    appointmentsBookedTransaction int;
    appointmentsCancelledTransaction int;
    nomPharmacyUpdateTransaction int;
    nomPharmacyCreateTransaction int;
    recordViewsDCRTransaction int;
    recordViewsSCRTransaction int;

    logResult bool;

BEGIN

    loginTransaction = audit.insertprocessduration(concat(usagePrefix, 'LoginTransaction'));
    INSERT INTO Counts ("SessionId", "Logins")
    SELECT "SessionId", count(*) AS "Logins"
    FROM "events"."LoginMetric" loginTable
    WHERE (loginTable."Timestamp" >= "startDate")
      AND (loginTable."Timestamp" < "endDate")
    GROUP BY "SessionId"
    ON CONFLICT("SessionId") DO UPDATE
        SET "Logins" = Excluded."Logins";
    logResult = audit.updateprocessduration(loginTransaction);

    prescriptionTransaction = audit.insertprocessduration(concat(usagePrefix, 'PrescriptionTransaction'));
    INSERT INTO Counts ("SessionId", "Prescriptions")
    SELECT "SessionId", count(*) AS "Prescriptions"
    FROM "events"."PrescriptionOrdersMetric" prescriptionsTable
    WHERE (prescriptionsTable."Timestamp" >= "startDate")
      AND (prescriptionsTable."Timestamp" < "endDate")
    GROUP BY "SessionId"
    ON CONFLICT("SessionId") DO UPDATE
        SET "Prescriptions" = Excluded."Prescriptions";
    logResult = audit.updateprocessduration(prescriptionTransaction);

    recordTransaction = audit.insertprocessduration(concat(usagePrefix, 'RecordTransaction'));
    INSERT INTO Counts ("SessionId", "RecordViews")
    SELECT "SessionId", count(*) AS "RecordViews"
    FROM "events"."MedicalRecordViewMetric" recordViewsTable
    WHERE (recordViewsTable."Timestamp" >= "startDate")
      AND (recordViewsTable."Timestamp" < "endDate")
    GROUP BY "SessionId"
    ON CONFLICT("SessionId") DO UPDATE
        SET "RecordViews" = Excluded."RecordViews";
    logResult = audit.updateprocessduration(recordTransaction);

    oDRegistrationTransaction = audit.insertprocessduration(concat(usagePrefix, 'ODRegistrationTransaction'));
    INSERT INTO Counts ("SessionId", "ODRegistrations")
    SELECT "SessionId", count(*) AS "ODRegistrations"
    FROM "events"."OrganDonationRegistrationCreateMetric" odRegistrationsTable
    WHERE (odRegistrationsTable."Timestamp" >= "startDate")
      AND (odRegistrationsTable."Timestamp" < "endDate")
    GROUP BY "SessionId"
    ON CONFLICT("SessionId") DO UPDATE
        SET "ODRegistrations" = Excluded."ODRegistrations";
    logResult = audit.updateprocessduration(oDRegistrationTransaction);

    oDWithdrawalTransaction = audit.insertprocessduration(concat(usagePrefix, 'ODWithdrawalTransaction'));
    INSERT INTO Counts ("SessionId", "ODWithdrawals")
    SELECT "SessionId", count(*) AS "ODWithdrawals"
    FROM "events"."OrganDonationRegistrationWithdrawMetric" odWithdrawalsTable
    WHERE (odWithdrawalsTable."Timestamp" >= "startDate")
      AND (odWithdrawalsTable."Timestamp" < "endDate")
    GROUP BY "SessionId"
    ON CONFLICT("SessionId") DO UPDATE
        SET "ODWithdrawals" = Excluded."ODWithdrawals";
    logResult = audit.updateprocessduration(oDWithdrawalTransaction);

    oDUpdateTransaction = audit.insertprocessduration(concat(usagePrefix, 'ODUpdateTransaction'));
    INSERT INTO Counts ("SessionId", "ODUpdates")
    SELECT "SessionId", count(*) AS "ODUpdates"
    FROM "events"."OrganDonationRegistrationUpdateMetric" odUpdatesTable
    WHERE (odUpdatesTable."Timestamp" >= "startDate")
      AND (odUpdatesTable."Timestamp" < "endDate")
    GROUP BY "SessionId"
    ON CONFLICT("SessionId") DO UPDATE
        SET "ODUpdates" = Excluded."ODUpdates";
    logResult = audit.updateprocessduration(oDUpdateTransaction);

    oDLookUpTransaction = audit.insertprocessduration(concat(usagePrefix, 'ODLookUpsTransaction'));
    INSERT INTO Counts ("SessionId", "ODLookups")
    SELECT "SessionId", count(*) AS "ODLookups"
    FROM "events"."OrganDonationRegistrationGetMetric" odLookupsTable
    WHERE (odLookupsTable."Timestamp" >= "startDate")
      AND (odLookupsTable."Timestamp" < "endDate")
    GROUP BY "SessionId"
    ON CONFLICT("SessionId") DO UPDATE
        SET "ODLookups" = Excluded."ODLookups";
    logResult = audit.updateprocessduration(oDLookUpTransaction);

    appointmentsBookedTransaction = audit.insertprocessduration(concat(usagePrefix, 'AppointmentsBookedTransaction'));
    INSERT INTO Counts ("SessionId", "AppointmentsBooked")
    SELECT "SessionId", count(*) AS "AppointmentsBooked"
    FROM "events"."AppointmentBookMetric" appointmentsBookedTable
    WHERE (appointmentsBookedTable."Timestamp" >= "startDate")
      AND (appointmentsBookedTable."Timestamp" < "endDate")
    GROUP BY "SessionId"
    ON CONFLICT("SessionId") DO UPDATE
        SET "AppointmentsBooked" = Excluded."AppointmentsBooked";
    logResult = audit.updateprocessduration(appointmentsBookedTransaction);

    appointmentsCancelledTransaction = audit.insertprocessduration(concat(usagePrefix, 'AppointmentsCancelledTransaction'));
    INSERT INTO Counts ("SessionId", "AppointmentsCancelled")
    SELECT "SessionId", count(*) AS "AppointmentsCancelled"
    FROM "events"."AppointmentCancelMetric" appointmentsCancelledTable
    WHERE (appointmentsCancelledTable."Timestamp" >= "startDate")
      AND (appointmentsCancelledTable."Timestamp" < "endDate")
    GROUP BY "SessionId"
    ON CONFLICT("SessionId") DO UPDATE
        SET "AppointmentsCancelled" = Excluded."AppointmentsCancelled";
    logResult = audit.updateprocessduration(appointmentsCancelledTransaction);

    nomPharmacyUpdateTransaction = audit.insertprocessduration(concat(usagePrefix, 'NomPharmacyUpdateTransaction'));
    INSERT INTO Counts ("SessionId", "NomPharmacyUpdate")
    SELECT "SessionId", count(*) AS "NomPharmacyUpdate"
    FROM "events"."NominatedPharmacyUpdateMetric" nomPharmacyUpdateTable
    WHERE (nomPharmacyUpdateTable."Timestamp" >= "startDate")
      AND (nomPharmacyUpdateTable."Timestamp" < "endDate")
    GROUP BY "SessionId"
    ON CONFLICT("SessionId") DO UPDATE
        SET "NomPharmacyUpdate" = Excluded."NomPharmacyUpdate";
    logResult = audit.updateprocessduration(nomPharmacyUpdateTransaction);

    nomPharmacyCreateTransaction = audit.insertprocessduration(concat(usagePrefix, 'NomPharmacyCreateTransaction'));
    INSERT INTO Counts ("SessionId", "NomPharmacyCreate")
    SELECT "SessionId", count(*) AS "NomPharmacyCreate"
    FROM "events"."NominatedPharmacyCreateMetric" nomPharmacyCreateTable
    WHERE (nomPharmacyCreateTable."Timestamp" >= "startDate")
      AND (nomPharmacyCreateTable."Timestamp" < "endDate")
    GROUP BY "SessionId"
    ON CONFLICT("SessionId") DO UPDATE
        SET "NomPharmacyCreate" = Excluded."NomPharmacyCreate";
    logResult = audit.updateprocessduration(nomPharmacyCreateTransaction);

    recordViewsDCRTransaction = audit.insertprocessduration(concat(usagePrefix, 'RecordViewsDCRTransaction'));
    INSERT INTO Counts ("SessionId", "RecordViewsDCR")
    SELECT "SessionId", count(*) AS "RecordViewsDCR"
    FROM "events"."MedicalRecordViewMetric" medicalRecordViewMetricTable
    WHERE medicalRecordViewMetricTable."HasDetailedRecordAccess" = true
        AND (medicalRecordViewMetricTable."Timestamp" >= "startDate")
        AND (medicalRecordViewMetricTable."Timestamp" < "endDate")
    GROUP BY "SessionId"
    ON CONFLICT("SessionId") DO UPDATE
        SET "RecordViewsDCR" = Excluded."RecordViewsDCR";
    logResult = audit.updateprocessduration(recordViewsDCRTransaction);

    recordViewsSCRTransaction = audit.insertprocessduration(concat(usagePrefix, 'RecordViewsSCRTransaction'));
    INSERT INTO Counts ("SessionId", "RecordViewsSCR")
    SELECT "SessionId", count(*) AS "RecordViewsSCR"
    FROM "events"."MedicalRecordViewMetric" medicalRecordViewMetricTable
    WHERE medicalRecordViewMetricTable."HasSummaryRecordAccess" = true
      AND (medicalRecordViewMetricTable."Timestamp" >= "startDate")
      AND (medicalRecordViewMetricTable."Timestamp" < "endDate")
    GROUP BY "SessionId"
    ON CONFLICT("SessionId") DO UPDATE
        SET "RecordViewsSCR" = Excluded."RecordViewsSCR";
    logResult = audit.updateprocessduration(recordViewsSCRTransaction);

END;
$$ LANGUAGE plpgsql;