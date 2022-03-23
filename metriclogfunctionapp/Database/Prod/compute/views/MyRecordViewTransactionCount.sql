
CREATE OR REPLACE VIEW compute."MyRecordViewTransactionCount" AS
SELECT
	transactionMetric."Timestamp"::date as "Date",
	login."OdsCode" as "OdsCode",
	count(*) as "Count"
FROM
    events."MedicalRecordViewMetric" transactionMetric
    LEFT JOIN events."LoginMetric" login
        ON transactionMetric."SessionId" = login."SessionId"
GROUP BY "Date", "OdsCode";

CALL perms.apply_etl_select_permissions('compute', 'MyRecordViewTransactionCount');
