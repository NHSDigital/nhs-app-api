
CREATE OR REPLACE VIEW compute."OrganDonationWithdrawTransactionCount" AS
SELECT
	transactionMetric."Timestamp"::date as "Date",
	"OdsCode",
	count(*) as "Count"
FROM events."OrganDonationRegistrationWithdrawMetric" transactionMetric
    LEFT JOIN events."LoginMetric" login
        ON transactionMetric."SessionId" = login."SessionId"
GROUP BY "Date", "OdsCode";

CALL perms.apply_etl_select_permissions('compute', 'OrganDonationWithdrawTransactionCount');
