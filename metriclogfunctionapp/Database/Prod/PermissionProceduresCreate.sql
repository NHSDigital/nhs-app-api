
CREATE SCHEMA IF NOT EXISTS perms;

CREATE OR REPLACE PROCEDURE perms.apply_etl_table_permissions(schemaname varchar, tablename varchar)
LANGUAGE plpgsql
SECURITY INVOKER
AS $$
DECLARE
    user varchar = current_user;
    qualified_name varchar = quote_ident(schemaname) || '.' || quote_ident(tablename);
    revoke_record record;
BEGIN
    IF EXISTS (
            SELECT 'tableowner'
            FROM pg_catalog.pg_tables p
            WHERE p.tablename = apply_etl_table_permissions.tablename
              AND p.schemaname = apply_etl_table_permissions.schemaname
              AND p.tableowner != user
        ) THEN
        EXECUTE 'ALTER TABLE ' || qualified_name || ' OWNER TO ' || user || ';';
    END IF;

    EXECUTE 'GRANT SELECT,INSERT ON ' || qualified_name || ' TO "' || current_database() || '_app";';
    EXECUTE 'GRANT SELECT ON ' || qualified_name || ' TO powerbi;';
    EXECUTE 'GRANT SELECT ON ' || qualified_name || ' TO "GRP-AzureNhsApp-Developers-DL";';
    EXECUTE 'GRANT SELECT ON ' || qualified_name || ' TO "GRP-AzureNhsApp-AnalyticsAdmins-DL";';

    FOR revoke_record IN
        SELECT 'REVOKE ' || g.privilege_type || ' ON ' || qualified_name || ' FROM ' || quote_ident(g.grantee) as revoke_statement
        FROM information_schema.role_table_grants g
        WHERE
            g.table_schema = schemaname AND
            g.table_name = tablename AND
            not (
                (g.privilege_type = 'SELECT' AND g.grantee = current_database() || '_app') OR
                (g.privilege_type = 'INSERT' AND g.grantee = current_database() || '_app') OR
                (g.privilege_type = 'SELECT' AND g.grantee = 'powerbi') OR
                (g.privilege_type = 'SELECT' AND g.grantee = 'GRP-AzureNhsApp-Developers-DL') OR
                (g.privilege_type = 'SELECT' AND g.grantee = 'GRP-AzureNhsApp-AnalyticsAdmins-DL')
            )
    LOOP
        EXECUTE revoke_record.revoke_statement;
    END LOOP;

END;
$$;

CREATE OR REPLACE PROCEDURE perms.apply_etl_select_permissions(schemaname varchar, tablename varchar)
LANGUAGE plpgsql
SECURITY INVOKER
AS $$
DECLARE
    user varchar = current_user;
    qualified_name varchar = quote_ident(schemaname) || '.' || quote_ident(tablename);
    revoke_record record;
BEGIN
    IF EXISTS (
            SELECT 'tableowner'
            FROM pg_catalog.pg_tables p
            WHERE p.tablename = apply_etl_select_permissions.tablename
              AND p.schemaname = apply_etl_select_permissions.schemaname
              AND p.tableowner != user
        ) THEN
        EXECUTE 'ALTER TABLE ' || qualified_name || ' OWNER TO ' || user || ';';
    END IF;

    EXECUTE 'GRANT SELECT ON ' || qualified_name || ' TO "' || current_database() || '_app";';
    EXECUTE 'GRANT SELECT ON ' || qualified_name || ' TO powerbi;';
    EXECUTE 'GRANT SELECT ON ' || qualified_name || ' TO "GRP-AzureNhsApp-Developers-DL";';
    EXECUTE 'GRANT SELECT ON ' || qualified_name || ' TO "GRP-AzureNhsApp-AnalyticsAdmins-DL";';
    FOR revoke_record IN
        SELECT 'REVOKE ' || g.privilege_type || ' ON ' || qualified_name || ' FROM ' || quote_ident(g.grantee) as revoke_statement
        FROM information_schema.role_table_grants g
        WHERE
            g.table_schema = schemaname AND
            g.table_name = tablename AND
            not (
                (g.privilege_type = 'SELECT' AND g.grantee = current_database() || '_app') OR
                (g.privilege_type = 'SELECT' AND g.grantee = 'powerbi') OR
                (g.privilege_type = 'SELECT' AND g.grantee = 'GRP-AzureNhsApp-Developers-DL') OR
                (g.privilege_type = 'SELECT' AND g.grantee = 'GRP-AzureNhsApp-AnalyticsAdmins-DL')
            )
    LOOP
        EXECUTE revoke_record.revoke_statement;
    END LOOP;
END;
$$;

CREATE OR REPLACE PROCEDURE perms.apply_legacy_insert_update_table_permissions(schemaname varchar, tablename varchar)
LANGUAGE plpgsql
SECURITY INVOKER
AS $$
DECLARE
    user varchar = current_user;
    qualified_name varchar = quote_ident(schemaname) || '.' || quote_ident(tablename);
    revoke_record record;
BEGIN
    IF EXISTS (
            SELECT 'tableowner'
            FROM pg_catalog.pg_tables p
            WHERE p.tablename = apply_legacy_insert_update_table_permissions.tablename
              AND p.schemaname = apply_legacy_insert_update_table_permissions.schemaname
              AND p.tableowner != user
        ) THEN
        EXECUTE 'ALTER TABLE ' || qualified_name || ' OWNER TO ' || user || ';';
    END IF;

    EXECUTE 'GRANT SELECT,INSERT,UPDATE ON ' || qualified_name || ' TO "' || current_database() || '_app";';
    EXECUTE 'GRANT SELECT ON ' || qualified_name || ' TO powerbi;';
    EXECUTE 'GRANT SELECT ON ' || qualified_name || ' TO "GRP-AzureNhsApp-Developers-DL";';
    EXECUTE 'GRANT SELECT ON ' || qualified_name || ' TO "GRP-AzureNhsApp-AnalyticsAdmins-DL";';

    FOR revoke_record IN
        SELECT 'REVOKE ' || g.privilege_type || ' ON ' || qualified_name || ' FROM ' || quote_ident(g.grantee) as revoke_statement
        FROM information_schema.role_table_grants g
        WHERE
            g.table_schema = schemaname AND
            g.table_name = tablename AND
            not (
                (g.privilege_type = 'SELECT' AND g.grantee = current_database() || '_app') OR
                (g.privilege_type = 'INSERT' AND g.grantee = current_database() || '_app') OR
                (g.privilege_type = 'UPDATE' AND g.grantee = current_database() || '_app') OR
                (g.privilege_type = 'SELECT' AND g.grantee = 'powerbi') OR
                (g.privilege_type = 'SELECT' AND g.grantee = 'GRP-AzureNhsApp-Developers-DL') OR
                (g.privilege_type = 'SELECT' AND g.grantee = 'GRP-AzureNhsApp-AnalyticsAdmins-DL')
            )
    LOOP
        EXECUTE revoke_record.revoke_statement;
    END LOOP;

END;
$$;

CREATE OR REPLACE PROCEDURE perms.apply_compute_table_permissions(schemaname varchar, tablename varchar)
LANGUAGE plpgsql
SECURITY INVOKER
AS $$
DECLARE
    user varchar = current_user;
    qualified_name varchar = quote_ident(schemaname) || '.' || quote_ident(tablename);
    revoke_record record;
BEGIN
    IF EXISTS (
            SELECT 'tableowner'
            FROM pg_catalog.pg_tables p
            WHERE p.tablename = apply_compute_table_permissions.tablename
              AND p.schemaname = apply_compute_table_permissions.schemaname
              AND p.tableowner != user
        ) THEN
        EXECUTE 'ALTER TABLE ' || qualified_name || ' OWNER TO ' || user || ';';
    END IF;

    EXECUTE 'GRANT SELECT,INSERT,UPDATE ON ' || qualified_name || ' TO "' || current_database() || '_app";';
    EXECUTE 'GRANT SELECT ON ' || qualified_name || ' TO powerbi;';
    EXECUTE 'GRANT SELECT ON ' || qualified_name || ' TO "GRP-AzureNhsApp-Developers-DL";';
    EXECUTE 'GRANT SELECT ON ' || qualified_name || ' TO "GRP-AzureNhsApp-AnalyticsAdmins-DL";';
    EXECUTE 'GRANT SELECT,INSERT,UPDATE ON ' || qualified_name || ' TO "AzureNhsAnalytics_DataFactory";';
    
    FOR revoke_record IN
        SELECT 'REVOKE ' || g.privilege_type || ' ON ' || qualified_name || ' FROM ' || quote_ident(g.grantee) as revoke_statement
        FROM information_schema.role_table_grants g
        WHERE
            g.table_schema = schemaname AND
            g.table_name = tablename AND
            not (
                (g.privilege_type = 'SELECT' AND g.grantee = current_database() || '_app') OR
                (g.privilege_type = 'INSERT' AND g.grantee = current_database() || '_app') OR
                (g.privilege_type = 'UPDATE' AND g.grantee = current_database() || '_app') OR
                (g.privilege_type = 'SELECT' AND g.grantee = 'powerbi') OR
                (g.privilege_type = 'SELECT' AND g.grantee = 'GRP-AzureNhsApp-Developers-DL') OR
                (g.privilege_type = 'SELECT' AND g.grantee = 'GRP-AzureNhsApp-AnalyticsAdmins-DL') OR
                (g.privilege_type = 'SELECT' AND g.grantee = 'AzureNhsAnalytics_DataFactory') OR
                (g.privilege_type = 'INSERT' AND g.grantee = 'AzureNhsAnalytics_DataFactory') OR
                (g.privilege_type = 'UPDATE' AND g.grantee = 'AzureNhsAnalytics_DataFactory')
            )
    LOOP
        EXECUTE revoke_record.revoke_statement;
    END LOOP;

END;
$$;

CREATE OR REPLACE PROCEDURE perms.apply_sequence_permissions(schemaname varchar, tablename varchar, keyname varchar)
LANGUAGE plpgsql
SECURITY INVOKER
AS $$
DECLARE
    sequencename varchar = quote_ident(schemaname) || '."' || tablename || '_' || keyname || '_seq"';
BEGIN

    EXECUTE 'GRANT USAGE ON SEQUENCE ' || sequencename || ' TO "' || current_database() || '_app";';
    EXECUTE 'GRANT USAGE ON SEQUENCE ' || sequencename || ' TO powerbi;';
    EXECUTE 'GRANT USAGE ON SEQUENCE ' || sequencename || ' TO "GRP-AzureNhsApp-Developers-DL";';
    EXECUTE 'GRANT USAGE ON SEQUENCE ' || sequencename || ' TO "GRP-AzureNhsApp-AnalyticsAdmins-DL";';
    EXECUTE 'GRANT USAGE ON SEQUENCE ' || sequencename || ' TO "AzureNhsAnalytics_DataFactory";';

END;
$$;