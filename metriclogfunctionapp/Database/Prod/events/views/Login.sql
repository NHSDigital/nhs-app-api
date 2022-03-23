
CREATE OR REPLACE VIEW events."Login" AS
SELECT
	"Timestamp",
	"OdsCode",
	"LoginId",
	"ProofLevel",
	cast(null as character varying(36)) "NhsNumber",
	"Referrer",
	"SessionId"
FROM events."LoginMetric";
