
CREATE OR REPLACE VIEW public."Daily_Transactions" AS
 SELECT t1."TransactionDate",
    t1."OdsCode",
    t1."Provider",
    t1."Registrations",
    t1."RegistrationsPYI",
    t1."AppointmentsBooked",
    t1."AppointmentsCancelled",
    t1."RepeatsOrdered",
    t1."RecordViews",
    t1."OdrRegistrations",
    t1."OdrUpdates",
    t1."OdrWithdrawals",
    t1."NominatedPharmacyChanges",
    t1."NdopStarts",
    t1."Logins",
    t1."OdrLookups",
    t1."TimeoutCount"
   FROM public."Historic_Transactions" t1
UNION
 SELECT t2."TransactionDate",
    t2."OdsCode",
    t2."Provider",
    t2."Registrations",
    t2."RegistrationsPYI",
    t2."AppointmentsBooked",
    t2."AppointmentsCancelled",
    t2."RepeatsOrdered",
    t2."RecordViews",
    t2."OdrRegistrations",
    t2."OdrUpdates",
    t2."OdrWithdrawals",
    t2."NominatedPharmacyChanges",
    t2."NdopStarts",
    t2."Logins",
    t2."OdrLookups",
    t2."TimeoutCount"
   FROM public."Transactions" t2
  WHERE (t2."TransactionDate" > '2020-04-26 00:00:00'::timestamp without time zone);

CALL perms.apply_etl_select_permissions('public', 'Daily_Transactions');
