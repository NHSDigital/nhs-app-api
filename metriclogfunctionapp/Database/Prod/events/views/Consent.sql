
CREATE OR REPLACE VIEW events."Consent" AS
    SELECT
        "Timestamp",
        "OdsCode",
        "LoginId",
        "ProofLevel"
    FROM
        events."ConsentMetric";
