
DROP PROCEDURE IF EXISTS perms.apply_etl_table_permissions;
DROP PROCEDURE IF EXISTS perms.apply_etl_select_permissions;
DROP PROCEDURE IF EXISTS perms.apply_legacy_insert_update_table_permissions;
DROP PROCEDURE IF EXISTS perms.apply_compute_table_permissions;
DROP PROCEDURE IF EXISTS perms.apply_sequence_permissions;

DROP SCHEMA IF EXISTS perms;

SELECT
    table_schema as "Schema",
    table_name as "Table",
    grantee as "User",
    privilege_type as "Perm",
    table_catalog,
    grantor,
    is_grantable,
    with_hierarchy
FROM information_schema.role_table_grants
WHERE grantee NOT IN ('pgsadmin', 'PUBLIC')
ORDER BY
    table_schema,
    table_name,
    grantee,
    privilege_type;
