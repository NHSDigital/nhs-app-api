CREATE OR REPLACE PROCEDURE compute.devicereferralcompute(
    "startDate" timestamp with time zone,
    "endDate" timestamp with time zone)
    LANGUAGE 'plpgsql'
AS $BODY$
BEGIN
    INSERT INTO compute."DailyDeviceReferralUsage" ("Date","DeviceOS","Referral","Users","Logins","RecordViewsDCR","RecordViewsSCR","Prescriptions","NomPharmacy","AppointmentsBooked","AppointmentsCancelled","ODRegistrations","ODWithdrawals","ReferralOrigin")
    SELECT
        wi."Timestamp"::date "Date",
        COALESCE(dd."DeviceOS",'Unknown') "DeviceOS",
        wi."Referrer" "Referral",
        COUNT(DISTINCT lm."LoginId") "Users",
        COUNT(wi."SessionId") "Logins",
        SUM(CASE WHEN mr."HasDetailedRecordAccess"='t' THEN 1 ELSE 0 END) AS "RecordViewsDCR",
        SUM(CASE WHEN mr."HasSummaryRecordAccess"='t' THEN 1 ELSE 0 END) AS "RecordViewsSCR"
        ,COUNT(po."Timestamp") AS "Prescriptions"
        ,COUNT(npc."Timestamp") + COUNT(npu."Timestamp") "NomPharmacy"
        ,COUNT(ab."Timestamp") AS "AppointmentsBooked"
        ,COUNT(ac."Timestamp") AS "AppointmentsCancelled"
        ,COUNT(odr."Timestamp") AS "ODRegistrations"
        ,COUNT(odw."Timestamp") AS "ODWithdrawals"
        ,wi."ReferrerOrigin" "ReferralOrigin"
    FROM "events"."WebIntegrationReferrals" wi
             LEFT JOIN events."Device" dd USING("SessionId")
             LEFT JOIN events."LoginMetric" lm USING("SessionId")
             LEFT JOIN events."MedicalRecordViewMetric" mr USING ("SessionId")
             LEFT JOIN events."AppointmentBookMetric" ab USING("SessionId")
             LEFT JOIN events."AppointmentCancelMetric" ac USING("SessionId")
             LEFT JOIN events."NominatedPharmacyCreateMetric" npc USING("SessionId")
             LEFT JOIN events."NominatedPharmacyUpdateMetric" npu USING("SessionId")
             LEFT JOIN events."PrescriptionOrdersMetric" po USING("SessionId")
             LEFT JOIN events."OrganDonationRegistrationCreateMetric" odr USING("SessionId")
             LEFT JOIN events."OrganDonationRegistrationWithdrawMetric" odw USING("SessionId")
    WHERE wi."Timestamp" >= "startDate" AND wi."Timestamp" < "endDate"
    GROUP BY wi."Timestamp"::date, dd."DeviceOS", wi."Referrer", wi."ReferrerOrigin"
    on conflict on constraint uniq_pkey do update
        SET "Date" = Excluded."Date",
            "DeviceOS" = Excluded."DeviceOS",
            "Referral" = Excluded."Referral",
            "ReferralOrigin" = Excluded."ReferralOrigin",
            "Users" = Excluded."Users",
            "Logins" = Excluded."Logins",
            "RecordViewsDCR" = Excluded."RecordViewsDCR",
            "RecordViewsSCR" = Excluded."RecordViewsSCR",
            "Prescriptions" = Excluded."Prescriptions",
            "NomPharmacy" = Excluded."NomPharmacy",
            "AppointmentsBooked" = Excluded."AppointmentsBooked",
            "AppointmentsCancelled" = Excluded."AppointmentsCancelled",
            "ODRegistrations" = Excluded."ODRegistrations",
            "ODWithdrawals" = Excluded."ODWithdrawals";
END;
$BODY$;

