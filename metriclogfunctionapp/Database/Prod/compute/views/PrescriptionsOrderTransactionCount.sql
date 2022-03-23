
CREATE OR REPLACE VIEW compute."PrescriptionsOrderTransactionCount" AS
SELECT
	transactionMetric."Timestamp"::date as "Date",
	login."OdsCode",
	count(*) as "Count"
FROM events."PrescriptionOrdersMetric" transactionMetric
    LEFT JOIN events."LoginMetric" login
        ON transactionMetric."SessionId" = login."SessionId"
GROUP BY "Date", "OdsCode";

CALL perms.apply_etl_select_permissions('compute', 'PrescriptionsOrderTransactionCount');
