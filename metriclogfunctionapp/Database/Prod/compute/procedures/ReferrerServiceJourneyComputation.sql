CREATE OR REPLACE PROCEDURE compute.ReferrerServiceJourneyComputation(
        "startDate" timestamp with time zone,
        "endDate" timestamp with time zone
    ) AS $$

DECLARE
thisSprocName varchar := 'ReferrerServiceJourneyComputation';
    usagePrefix varchar := format('ReferrerServiceJourney_');
    referrerId int;
    recordViewsForPeriodInsert int;
    prescriptionsForPeriodInsert int;
    oDRegistrationsForPeriodInsert int;
    oDWithdrawalsForPeriodInsert int;
    oDUpdatesForPeriodInsert int;
    oDLookupsForPeriodInsert int;
    nomPharmacyUpdateForPeriodInsert int;
    nomPharmacyCreateForPeriodInsert int;
    appointmentsBookedForPeriodInsert int;
    appointmentsCancelledForPeriodInsert int;
    recordViewsDcrForPeriodInsert int;
    recordViewsScrForPeriodInsert int;
    silverIntegrationJumpOffsForPeriodInsert int;
    covidPassJumpOffsForPeriodInsert int;
    referrerServiceJourney int;
    logResult bool;

BEGIN
    referrerId = audit.insertprocessduration(thisSprocName);

    -- RecordViews
    -- Create temp table to build up the counts of RecordViews
    CREATE TEMP TABLE IF NOT EXISTS RecordViewsForPeriod (
        "Date" date,
        "ReferrerId" text,
        "UserCount" integer DEFAULT 0);

    -- Gather the referrer service journeys for today's RecordViews
    recordViewsForPeriodInsert = audit.insertprocessduration(concat(usagePrefix, 'RecordViewsForPeriod_Insert'));
    INSERT INTO RecordViewsForPeriod ("Date", "ReferrerId", "UserCount")
        SELECT Date (wir."Timestamp") as "DateOfTimestamp", wir."Referrer", COUNT (*) as "UserCount"
        FROM events."WebIntegrationReferrals" wir
        INNER JOIN events."MedicalRecordViewMetric" t1 on wir."SessionId" = t1."SessionId"
        WHERE (wir."Timestamp" >= "startDate")
          AND (wir."Timestamp" < "endDate")
        GROUP By "DateOfTimestamp", wir."Referrer";
    logResult = audit.updateprocessduration(recordViewsForPeriodInsert);

    -- Insert the result from the temp tables into the ReferrerServiceJourney table
    referrerServiceJourney = audit.insertprocessduration('ReferrerServiceJourney_Insert');
    INSERT INTO "compute"."ReferrerServiceJourney" ("Date", "ReferrerId", "RecordViews")
        SELECT t1."Date", t1."ReferrerId", SUM(t1."UserCount") as "RecordViews"
        FROM RecordViewsForPeriod t1
        GROUP BY t1."Date", t1."ReferrerId"
    ON CONFLICT("Date", "ReferrerId") DO
        UPDATE SET "RecordViews" = Excluded."RecordViews";
    logResult = audit.updateprocessduration(referrerServiceJourney);

    -- Delete temp table
    DROP TABLE IF EXISTS RecordViewsForPeriod;

    -- Prescriptions
    -- Create temp table to build up the counts of Prescriptions
    CREATE TEMP TABLE IF NOT EXISTS PrescriptionsForPeriod (
        "Date" date,
        "ReferrerId" text,
        "UserCount" integer DEFAULT 0);

    -- Gather the referrer service journeys for today's Prescriptions
    prescriptionsForPeriodInsert = audit.insertprocessduration(concat(usagePrefix, 'PrescriptionsForPeriod_Insert'));
    INSERT INTO PrescriptionsForPeriod ("Date", "ReferrerId", "UserCount")
        SELECT Date(wir."Timestamp") as "DateOfTimestamp", wir."Referrer", COUNT(*) as "UserCount"
        FROM events."WebIntegrationReferrals" wir
        INNER JOIN events."PrescriptionOrdersMetric" t1 on wir."SessionId" = t1."SessionId"
        WHERE (wir."Timestamp" >= "startDate")
          AND (wir."Timestamp" < "endDate")
        GROUP By "DateOfTimestamp", wir."Referrer";
    logResult = audit.updateprocessduration(prescriptionsForPeriodInsert);

    -- Insert the result from the temp tables into the ReferrerServiceJourney table
    referrerServiceJourney = audit.insertprocessduration('ReferrerServiceJourney_Insert');
    INSERT INTO "compute"."ReferrerServiceJourney" ("Date", "ReferrerId", "Prescriptions")
        SELECT t1."Date", t1."ReferrerId", SUM(t1."UserCount") as "Prescriptions"
        FROM PrescriptionsForPeriod t1
        GROUP BY t1."Date", t1."ReferrerId"
    ON CONFLICT("Date", "ReferrerId") DO
        UPDATE SET "Prescriptions" = Excluded."Prescriptions";
    logResult = audit.updateprocessduration(referrerServiceJourney);

    -- Delete temp table
    DROP TABLE IF EXISTS PrescriptionsForPeriod;

    -- OdRegistrations
    -- Create temp table to build up the counts of OdRegistrations
    CREATE TEMP TABLE IF NOT EXISTS OdRegistrationsForPeriod (
        "Date" date,
        "ReferrerId" text,
        "UserCount" integer DEFAULT 0);

    -- Gather the referrer service journeys for today's OdRegistrations
    odRegistrationsForPeriodInsert = audit.insertprocessduration(concat(usagePrefix, 'OdRegistrationsForPeriod_Insert'));
    INSERT INTO OdRegistrationsForPeriod ("Date", "ReferrerId", "UserCount")
        SELECT Date(wir."Timestamp") as "DateOfTimestamp", wir."Referrer", COUNT(*) as "UserCount"
        FROM events."WebIntegrationReferrals" wir
        INNER JOIN events."OrganDonationRegistrationCreateMetric" t1 on wir."SessionId" = t1."SessionId"
        WHERE (wir."Timestamp" >= "startDate")
          AND (wir."Timestamp" < "endDate")
        GROUP By "DateOfTimestamp", wir."Referrer";
    logResult = audit.updateprocessduration(odRegistrationsForPeriodInsert);

    -- Insert the result from the temp tables into the ReferrerServiceJourney table
    referrerServiceJourney = audit.insertprocessduration('ReferrerServiceJourney_Insert');
    INSERT INTO "compute"."ReferrerServiceJourney" ("Date", "ReferrerId", "OdRegistrations")
        SELECT t1."Date", t1."ReferrerId", SUM(t1."UserCount") as "OdRegistrations"
        FROM OdRegistrationsForPeriod t1
        GROUP BY t1."Date", t1."ReferrerId"
    ON CONFLICT("Date", "ReferrerId") DO
        UPDATE SET "OdRegistrations" = Excluded."OdRegistrations";
    logResult = audit.updateprocessduration(referrerServiceJourney);

    -- Delete temp table
    DROP TABLE IF EXISTS OdRegistrationsForPeriod;

    -- OdWithdrawals
    -- Create temp table to build up the counts of OdWithdrawals
    CREATE TEMP TABLE IF NOT EXISTS OdWithdrawalsForPeriod (
        "Date" date,
        "ReferrerId" text,
        "UserCount" integer DEFAULT 0);

    -- Gather the referrer service journeys for today's OdWithdrawals
    odWithdrawalsForPeriodInsert = audit.insertprocessduration(concat(usagePrefix, 'OdWithdrawalsForPeriod_Insert'));
    INSERT INTO OdWithdrawalsForPeriod ("Date", "ReferrerId", "UserCount")
        SELECT Date(wir."Timestamp") as "DateOfTimestamp", wir."Referrer", COUNT(*) as "UserCount"
        FROM events."WebIntegrationReferrals" wir
        INNER JOIN events."OrganDonationRegistrationWithdrawMetric" t1 on wir."SessionId" = t1."SessionId"
        WHERE (wir."Timestamp" >= "startDate")
          AND (wir."Timestamp" < "endDate")
        GROUP By "DateOfTimestamp", wir."Referrer";
    logResult = audit.updateprocessduration(odWithdrawalsForPeriodInsert);

    -- Insert the result from the temp tables into the ReferrerServiceJourney table
    referrerServiceJourney = audit.insertprocessduration('ReferrerServiceJourney_Insert');
    INSERT INTO "compute"."ReferrerServiceJourney" ("Date", "ReferrerId", "OdWithdrawals")
        SELECT t1."Date", t1."ReferrerId", SUM(t1."UserCount") as "OdWithdrawals"
        FROM OdWithdrawalsForPeriod t1
        GROUP BY t1."Date", t1."ReferrerId"
    ON CONFLICT("Date", "ReferrerId") DO
        UPDATE SET "OdWithdrawals" = Excluded."OdWithdrawals";
    logResult = audit.updateprocessduration(referrerServiceJourney);

    -- Delete temp table
    DROP TABLE IF EXISTS OdWithdrawalsForPeriod;

    -- OdUpdates
    -- Create temp table to build up the counts of OdUpdates
    CREATE TEMP TABLE IF NOT EXISTS OdUpdatesForPeriod (
        "Date" date,
        "ReferrerId" text,
        "UserCount" integer DEFAULT 0);

    -- Gather the referrer service journeys for today's OdUpdates
    odUpdatesForPeriodInsert = audit.insertprocessduration(concat(usagePrefix, 'OdUpdatesForPeriod_Insert'));
    INSERT INTO OdUpdatesForPeriod ("Date", "ReferrerId", "UserCount")
        SELECT Date(wir."Timestamp") as "DateOfTimestamp", wir."Referrer", COUNT(*) as "UserCount"
        FROM events."WebIntegrationReferrals" wir
        INNER JOIN events."OrganDonationRegistrationUpdateMetric" t1 on wir."SessionId" = t1."SessionId"
        WHERE (wir."Timestamp" >= "startDate")
          AND (wir."Timestamp" < "endDate")
        GROUP By "DateOfTimestamp", wir."Referrer";
    logResult = audit.updateprocessduration(odUpdatesForPeriodInsert);

    -- Insert the result from the temp tables into the ReferrerServiceJourney table
    referrerServiceJourney = audit.insertprocessduration('ReferrerServiceJourney_Insert');
    INSERT INTO "compute"."ReferrerServiceJourney" ("Date", "ReferrerId", "OdUpdates")
        SELECT t1."Date", t1."ReferrerId", SUM(t1."UserCount") as "OdUpdates"
        FROM OdUpdatesForPeriod t1
        GROUP BY t1."Date", t1."ReferrerId"
    ON CONFLICT("Date", "ReferrerId") DO
        UPDATE SET "OdUpdates" = Excluded."OdUpdates";
    logResult = audit.updateprocessduration(referrerServiceJourney);

    -- Delete temp table
    DROP TABLE IF EXISTS OdUpdatesForPeriod;

    -- OdLookups
    -- Create temp table to build up the counts of OdLookups
    CREATE TEMP TABLE IF NOT EXISTS OdLookupsForPeriod (
        "Date" date,
        "ReferrerId" text,
        "UserCount" integer DEFAULT 0);

    -- Gather the referrer service journeys for today's OdLookups
    odLookupsForPeriodInsert = audit.insertprocessduration(concat(usagePrefix, 'OdLookupsForPeriod_Insert'));
    INSERT INTO OdLookupsForPeriod ("Date", "ReferrerId", "UserCount")
        SELECT Date(wir."Timestamp") as "DateOfTimestamp", wir."Referrer", COUNT(*) as "UserCount"
        FROM events."WebIntegrationReferrals" wir
        INNER JOIN events."OrganDonationRegistrationGetMetric" t1 on wir."SessionId" = t1."SessionId"
        WHERE (wir."Timestamp" >= "startDate")
          AND (wir."Timestamp" < "endDate")
        GROUP By "DateOfTimestamp", wir."Referrer";
    logResult = audit.updateprocessduration(odLookupsForPeriodInsert);

    -- Insert the result from the temp tables into the ReferrerServiceJourney table
    referrerServiceJourney = audit.insertprocessduration('ReferrerServiceJourney_Insert');
    INSERT INTO "compute"."ReferrerServiceJourney" ("Date", "ReferrerId", "OdLookups")
        SELECT t1."Date", t1."ReferrerId", SUM(t1."UserCount") as "OdLookups"
        FROM OdLookupsForPeriod t1
        GROUP BY t1."Date", t1."ReferrerId"
    ON CONFLICT("Date", "ReferrerId") DO
        UPDATE SET "OdLookups" = Excluded."OdLookups";
    logResult = audit.updateprocessduration(referrerServiceJourney);

    -- Delete temp table
    DROP TABLE IF EXISTS OdLookupsForPeriod;

    -- NomPharmacyUpdate
    -- Create temp table to build up the counts of NomPharmacyUpdate
    CREATE TEMP TABLE IF NOT EXISTS NomPharmacyUpdateForPeriod (
        "Date" date,
        "ReferrerId" text,
        "UserCount" integer DEFAULT 0);

    -- Gather the referrer service journeys for today's NomPharmacyUpdate
    nomPharmacyUpdateForPeriodInsert = audit.insertprocessduration(concat(usagePrefix, 'NomPharmacyUpdateForPeriod_Insert'));
    INSERT INTO NomPharmacyUpdateForPeriod ("Date", "ReferrerId", "UserCount")
        SELECT Date(wir."Timestamp") as "DateOfTimestamp", wir."Referrer", COUNT(*) as "UserCount"
        FROM events."WebIntegrationReferrals" wir
        INNER JOIN events."NominatedPharmacyUpdateMetric" t1 on wir."SessionId" = t1."SessionId"
        WHERE (wir."Timestamp" >= "startDate")
          AND (wir."Timestamp" < "endDate")
        GROUP By "DateOfTimestamp", wir."Referrer";
    logResult = audit.updateprocessduration(nomPharmacyUpdateForPeriodInsert);

    -- Insert the result from the temp tables into the ReferrerServiceJourney table
    referrerServiceJourney = audit.insertprocessduration('ReferrerServiceJourney_Insert');
    INSERT INTO "compute"."ReferrerServiceJourney" ("Date", "ReferrerId", "NomPharmacyUpdate")
        SELECT t1."Date", t1."ReferrerId", SUM(t1."UserCount") as "NomPharmacyUpdate"
        FROM NomPharmacyUpdateForPeriod t1
        GROUP BY t1."Date", t1."ReferrerId"
    ON CONFLICT("Date", "ReferrerId") DO
        UPDATE SET "NomPharmacyUpdate" = Excluded."NomPharmacyUpdate";
    logResult = audit.updateprocessduration(referrerServiceJourney);

    -- Delete temp table
    DROP TABLE IF EXISTS NomPharmacyUpdateForPeriod;

    -- NomPharmacyCreate
    -- Create temp table to build up the counts of NomPharmacyCreate
    CREATE TEMP TABLE IF NOT EXISTS NomPharmacyCreateForPeriod (
        "Date" date,
        "ReferrerId" text,
        "UserCount" integer DEFAULT 0);

    -- Gather the referrer service journeys for today's NomPharmacyCreate
    nomPharmacyCreateForPeriodInsert = audit.insertprocessduration(concat(usagePrefix, 'NomPharmacyCreateForPeriod_Insert'));
    INSERT INTO NomPharmacyCreateForPeriod ("Date", "ReferrerId", "UserCount")
        SELECT Date(wir."Timestamp") as "DateOfTimestamp", wir."Referrer", COUNT(*) as "UserCount"
        FROM events."WebIntegrationReferrals" wir
        INNER JOIN events."NominatedPharmacyCreateMetric" t1 on wir."SessionId" = t1."SessionId"
        WHERE (wir."Timestamp" >= "startDate")
          AND (wir."Timestamp" < "endDate")
        GROUP By "DateOfTimestamp", wir."Referrer";
    logResult = audit.updateprocessduration(nomPharmacyCreateForPeriodInsert);

    -- Insert the result from the temp tables into the ReferrerServiceJourney table
    referrerServiceJourney = audit.insertprocessduration('ReferrerServiceJourney_Insert');
    INSERT INTO "compute"."ReferrerServiceJourney" ("Date", "ReferrerId", "NomPharmacyCreate")
        SELECT t1."Date", t1."ReferrerId", SUM(t1."UserCount") as "NomPharmacyCreate"
        FROM NomPharmacyCreateForPeriod t1
        GROUP BY t1."Date", t1."ReferrerId"
    ON CONFLICT("Date", "ReferrerId") DO
        UPDATE SET "NomPharmacyCreate" = Excluded."NomPharmacyCreate";
    logResult = audit.updateprocessduration(referrerServiceJourney);

    -- Delete temp table
    DROP TABLE IF EXISTS NomPharmacyCreateForPeriod;

    -- AppointmentsBooked
    -- Create temp table to build up the counts of AppointmentsBooked
    CREATE TEMP TABLE IF NOT EXISTS AppointmentsBookedForPeriod (
        "Date" date,
        "ReferrerId" text,
        "UserCount" integer DEFAULT 0);

    -- Gather the referrer service journeys for today's AppointmentsBooked
    appointmentsBookedForPeriodInsert = audit.insertprocessduration(concat(usagePrefix, 'AppointmentsBookedForPeriod_Insert'));
    INSERT INTO AppointmentsBookedForPeriod ("Date", "ReferrerId", "UserCount")
        SELECT Date(wir."Timestamp") as "DateOfTimestamp", wir."Referrer", COUNT(*) as "UserCount"
        FROM events."WebIntegrationReferrals" wir
        INNER JOIN events."AppointmentBookMetric" t1 on wir."SessionId" = t1."SessionId"
        WHERE (wir."Timestamp" >= "startDate")
          AND (wir."Timestamp" < "endDate")
        GROUP By "DateOfTimestamp", wir."Referrer";
    logResult = audit.updateprocessduration(appointmentsBookedForPeriodInsert);

    -- Insert the result from the temp tables into the ReferrerServiceJourney table
    referrerServiceJourney = audit.insertprocessduration('ReferrerServiceJourney_Insert');
    INSERT INTO "compute"."ReferrerServiceJourney" ("Date", "ReferrerId", "AppointmentsBooked")
        SELECT t1."Date", t1."ReferrerId", SUM(t1."UserCount") as "AppointmentsBooked"
        FROM AppointmentsBookedForPeriod t1
        GROUP BY t1."Date", t1."ReferrerId"
    ON CONFLICT("Date", "ReferrerId") DO
        UPDATE SET "AppointmentsBooked" = Excluded."AppointmentsBooked";
    logResult = audit.updateprocessduration(referrerServiceJourney);

    -- Delete temp table
    DROP TABLE IF EXISTS AppointmentsBookedForPeriod;

    -- AppointmentsCancelled
    -- Create temp table to build up the counts of AppointmentsCancelled
    CREATE TEMP TABLE IF NOT EXISTS AppointmentsCancelledForPeriod (
        "Date" date,
        "ReferrerId" text,
        "UserCount" integer DEFAULT 0);

    -- Gather the referrer service journeys for today's AppointmentsCancelled
    appointmentsCancelledForPeriodInsert = audit.insertprocessduration(concat(usagePrefix, 'AppointmentsCancelledForPeriod_Insert'));
    INSERT INTO AppointmentsCancelledForPeriod ("Date", "ReferrerId", "UserCount")
        SELECT Date(wir."Timestamp") as "DateOfTimestamp", wir."Referrer", COUNT(*) as "UserCount"
        FROM events."WebIntegrationReferrals" wir
        INNER JOIN events."AppointmentCancelMetric" t1 on wir."SessionId" = t1."SessionId"
        WHERE (wir."Timestamp" >= "startDate")
          AND (wir."Timestamp" < "endDate")
        GROUP By "DateOfTimestamp", wir."Referrer";
    logResult = audit.updateprocessduration(appointmentsCancelledForPeriodInsert);

    -- Insert the result from the temp tables into the ReferrerServiceJourney table
    referrerServiceJourney = audit.insertprocessduration('ReferrerServiceJourney_Insert');
    INSERT INTO "compute"."ReferrerServiceJourney" ("Date", "ReferrerId", "AppointmentsCancelled")
        SELECT t1."Date", t1."ReferrerId", SUM(t1."UserCount") as "AppointmentsCancelled"
        FROM AppointmentsCancelledForPeriod t1
        GROUP BY t1."Date", t1."ReferrerId"
    ON CONFLICT("Date", "ReferrerId") DO
        UPDATE SET "AppointmentsCancelled" = Excluded."AppointmentsCancelled";
    logResult = audit.updateprocessduration(referrerServiceJourney);

    -- Delete temp table
    DROP TABLE IF EXISTS AppointmentsCancelledForPeriod;

    -- RecordViewsDcr
    -- Create temp table to build up the counts of RecordViewsDcr
    CREATE TEMP TABLE IF NOT EXISTS RecordViewsDcrForPeriod (
        "Date" date,
        "ReferrerId" text,
        "UserCount" integer DEFAULT 0);

    -- Gather the referrer service journeys for today's RecordViewsDcr
    recordViewsDcrForPeriodInsert = audit.insertprocessduration(concat(usagePrefix, 'RecordViewsDcrForPeriod_Insert'));
    INSERT INTO RecordViewsDcrForPeriod ("Date", "ReferrerId", "UserCount")
        SELECT Date(wir."Timestamp") as "DateOfTimestamp", wir."Referrer", COUNT(*) as "UserCount"
        FROM events."WebIntegrationReferrals" wir
        INNER JOIN events."MedicalRecordViewMetric" t1 on wir."SessionId" = t1."SessionId"
        WHERE (wir."Timestamp" >= "startDate")
          AND (wir."Timestamp" < "endDate")
          AND (t1."HasDetailedRecordAccess" IS TRUE)
        GROUP By "DateOfTimestamp", wir."Referrer";
    logResult = audit.updateprocessduration(recordViewsDcrForPeriodInsert);

    -- Insert the result from the temp tables into the ReferrerServiceJourney table
    referrerServiceJourney = audit.insertprocessduration('ReferrerServiceJourney_Insert');
    INSERT INTO "compute"."ReferrerServiceJourney" ("Date", "ReferrerId", "RecordViewsDcr")
        SELECT t1."Date", t1."ReferrerId", SUM(t1."UserCount") AS "RecordViewsDcr"
        FROM RecordViewsDcrForPeriod t1
        GROUP BY t1."Date", t1."ReferrerId"
    ON CONFLICT("Date", "ReferrerId") DO
        UPDATE SET "RecordViewsDcr" = Excluded."RecordViewsDcr";
    logResult = audit.updateprocessduration(referrerServiceJourney);

    -- Delete temp table
    DROP TABLE IF EXISTS RecordViewsDcrForPeriod;

    -- RecordViewsScr
    -- Create temp table to build up the counts of RecordViewsScr
    CREATE TEMP TABLE IF NOT EXISTS RecordViewsScrForPeriod (
        "Date" date,
        "ReferrerId" text,
        "UserCount" integer DEFAULT 0);

    -- Gather the referrer service journeys for today's RecordViewsScr
    recordViewsScrForPeriodInsert = audit.insertprocessduration(concat(usagePrefix, 'RecordViewsScrForPeriod_Insert'));
    INSERT INTO RecordViewsScrForPeriod ("Date", "ReferrerId", "UserCount")
        SELECT Date(wir."Timestamp") as "DateOfTimestamp", wir."Referrer", COUNT(*) as "UserCount"
        FROM events."WebIntegrationReferrals" wir
        INNER JOIN events."MedicalRecordViewMetric" t1 on wir."SessionId" = t1."SessionId"
        WHERE (wir."Timestamp" >= "startDate")
          AND (wir."Timestamp" < "endDate")
          AND (t1."HasSummaryRecordAccess" IS TRUE)
        GROUP By "DateOfTimestamp", wir."Referrer";
    logResult = audit.updateprocessduration(recordViewsScrForPeriodInsert);

    -- Insert the result from the temp tables into the ReferrerServiceJourney table
    referrerServiceJourney = audit.insertprocessduration('ReferrerServiceJourney_Insert');
    INSERT INTO "compute"."ReferrerServiceJourney" ("Date", "ReferrerId", "RecordViewsScr")
        SELECT t1."Date", t1."ReferrerId", SUM(t1."UserCount") as "RecordViewsScr"
        FROM RecordViewsScrForPeriod t1
        GROUP BY t1."Date", t1."ReferrerId"
    ON CONFLICT("Date", "ReferrerId") DO
        UPDATE SET "RecordViewsScr" = Excluded."RecordViewsScr";
    logResult = audit.updateprocessduration(referrerServiceJourney);

    -- Delete temp table
    DROP TABLE IF EXISTS RecordViewsScrForPeriod;

    -- SilverIntegrationJumpOffs
    -- Create temp table to build up the counts of SilverIntegrationJumpOffs
    CREATE TEMP TABLE IF NOT EXISTS SilverIntegrationJumpOffsForPeriod (
        "Date" date,
        "ReferrerId" text,
        "UserCount" integer DEFAULT 0);

    -- Gather the referrer service journeys for today's SilverIntegrationJumpOffs
    silverIntegrationJumpOffsForPeriodInsert = audit.insertprocessduration(concat(usagePrefix, 'SilverIntegrationJumpOffsForPeriod_Insert'));
    INSERT INTO SilverIntegrationJumpOffsForPeriod ("Date", "ReferrerId", "UserCount")
        SELECT Date(wir."Timestamp") as "DateOfTimestamp", wir."Referrer", COUNT(*) as "UserCount"
        FROM events."WebIntegrationReferrals" wir
        INNER JOIN events."SilverIntegrationJumpOffMetric" t1 on wir."SessionId" = t1."SessionId"
        WHERE (wir."Timestamp" >= "startDate")
          AND (wir."Timestamp" < "endDate")
          AND (t1."ProviderName" <> 'the Department of Health and Social Care')
        GROUP By "DateOfTimestamp", wir."Referrer";
    logResult = audit.updateprocessduration(silverIntegrationJumpOffsForPeriodInsert);

    -- Insert the result from the temp tables into the ReferrerServiceJourney table
    referrerServiceJourney = audit.insertprocessduration('ReferrerServiceJourney_Insert');
    INSERT INTO "compute"."ReferrerServiceJourney" ("Date", "ReferrerId", "SilverIntegrationJumpOffs")
        SELECT t1."Date", t1."ReferrerId", SUM(t1."UserCount") as "SilverIntegrationJumpOffs"
        FROM SilverIntegrationJumpOffsForPeriod t1
        GROUP BY t1."Date", t1."ReferrerId"
    ON CONFLICT("Date", "ReferrerId") DO
        UPDATE SET "SilverIntegrationJumpOffs" = Excluded."SilverIntegrationJumpOffs";
    logResult = audit.updateprocessduration(referrerServiceJourney);

    -- Delete temp table
    DROP TABLE IF EXISTS SilverIntegrationJumpOffsForPeriod;

    -- CovidPassJumpOffs
    -- Create temp table to build up the counts of CovidPassJumpOffs
    CREATE TEMP TABLE IF NOT EXISTS CovidPassJumpOffsForPeriod (
        "Date" date,
        "ReferrerId" text,
        "UserCount" integer DEFAULT 0);--

    -- Gather the referrer service journeys for today's CovidPassJumpOffs
    covidPassJumpOffsForPeriodInsert = audit.insertprocessduration(concat(usagePrefix, 'CovidPassJumpOffsForPeriod_Insert'));
    INSERT INTO CovidPassJumpOffsForPeriod ("Date", "ReferrerId", "UserCount")
        SELECT Date(wir."Timestamp") as "DateOfTimestamp", wir."Referrer", COUNT(*) as "UserCount"
        FROM events."WebIntegrationReferrals" wir
        INNER JOIN events."SilverIntegrationJumpOffMetric" t1 on wir."SessionId" = t1."SessionId"
        WHERE (wir."Timestamp" >= "startDate")
          AND (wir."Timestamp" < "endDate")
          AND (t1."ProviderName" = 'the Department of Health and Social Care')
        GROUP By "DateOfTimestamp", wir."Referrer";
    logResult = audit.updateprocessduration(covidPassJumpOffsForPeriodInsert);

     -- Insert the result from the temp tables into the ReferrerServiceJourney table
    referrerServiceJourney = audit.insertprocessduration('ReferrerServiceJourney_Insert');
    INSERT INTO "compute"."ReferrerServiceJourney" ("Date", "ReferrerId", "CovidPassJumpOffs")
        SELECT t1."Date", t1."ReferrerId", SUM(t1."UserCount") as "CovidPassJumpOffs"
        FROM CovidPassJumpOffsForPeriod t1
        GROUP BY t1."Date", t1."ReferrerId"
    ON CONFLICT("Date", "ReferrerId") DO
        UPDATE SET "CovidPassJumpOffs" = Excluded."CovidPassJumpOffs";
    logResult = audit.updateprocessduration(referrerServiceJourney);

     -- Delete temp table
    DROP TABLE IF EXISTS CovidPassJumpOffsForPeriod;

    logResult = audit.updateprocessduration(referrerId);

END;

$$ LANGUAGE plpgsql;
