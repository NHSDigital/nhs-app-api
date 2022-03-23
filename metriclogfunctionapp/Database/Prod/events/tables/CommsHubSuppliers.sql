CREATE TABLE IF NOT EXISTS events."CommsHubSuppliers" (
      "Supplier" character varying(50) NOT NULL,
      "SupplierId" character varying(36) NOT NULL UNIQUE
);

INSERT INTO events."CommsHubSuppliers" ("Supplier","SupplierId")
SELECT
    'NHS App/Internal','278d3b75-3498-4d68-8991-506d0006e46f'
WHERE NOT EXISTS (
        SELECT 1 FROM events."CommsHubSuppliers" WHERE "Supplier" = 'NHS App/Internal' AND "SupplierId" = '278d3b75-3498-4d68-8991-506d0006e46f'
    );

INSERT INTO events."CommsHubSuppliers" ("Supplier","SupplierId")
SELECT
    'NHS App/Internal','5c33276f-5ff5-48e3-ba74-d7389f99541c'
WHERE NOT EXISTS (
        SELECT 1 FROM events."CommsHubSuppliers" WHERE "Supplier" = 'NHS App/Internal' AND "SupplierId" = '5c33276f-5ff5-48e3-ba74-d7389f99541c'
    );

INSERT INTO events."CommsHubSuppliers" ("Supplier","SupplierId")
SELECT
    'Mjog','409a0887-a946-4884-9796-45296a053192'
WHERE NOT EXISTS (
    SELECT 1 FROM events."CommsHubSuppliers" WHERE "Supplier" = 'Mjog' AND "SupplierId" = '409a0887-a946-4884-9796-45296a053192'
    );

CALL perms.apply_etl_select_permissions('events', 'CommsHubSuppliers');