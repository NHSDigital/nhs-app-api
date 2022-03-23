
CREATE OR REPLACE VIEW compute."LoginTransactionCount" AS
SELECT
	"Timestamp"::date as "Date",
	"OdsCode",
	count(*) as "Count"
FROM events."LoginMetric"
GROUP BY "Date", "OdsCode";

CALL perms.apply_etl_select_permissions('compute', 'LoginTransactionCount');
