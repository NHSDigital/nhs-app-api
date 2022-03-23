
CREATE TABLE IF NOT EXISTS compute."TestOdsCodes" (
    "OdsCode" character varying NOT NULL UNIQUE
);

INSERT INTO compute."TestOdsCodes" ("OdsCode")
VALUES
    ('A12355'),
    ('A21410'),
    ('A29928')
ON CONFLICT("OdsCode") DO NOTHING;

CALL perms.apply_etl_select_permissions('compute', 'TestOdsCodes');
