
DO $$
BEGIN
    IF NOT EXISTS (SELECT FROM pg_catalog.pg_user WHERE usename = current_database() || '_app') THEN
        EXECUTE 'CREATE USER "' || current_database() || '_app"';
    END IF;
    EXECUTE 'GRANT USAGE ON SCHEMA audit to "' || current_database() || '_app";';
    EXECUTE 'GRANT USAGE ON SCHEMA compute to "' || current_database() || '_app";';
    EXECUTE 'GRANT USAGE ON SCHEMA events to "' || current_database() || '_app";';
    EXECUTE 'GRANT USAGE ON SCHEMA public to "' || current_database() || '_app";';
    EXECUTE 'GRANT USAGE ON SCHEMA reports to "' || current_database() || '_app";';
    EXECUTE 'GRANT USAGE ON SCHEMA ref to "' || current_database() || '_app";';

    IF NOT EXISTS (SELECT FROM pg_catalog.pg_user WHERE usename = 'powerbi') THEN
        CREATE USER powerbi;
    END IF;
    GRANT USAGE ON SCHEMA compute to powerbi;
    GRANT USAGE ON SCHEMA events to powerbi;
    GRANT USAGE ON SCHEMA public to powerbi;
    GRANT USAGE ON SCHEMA reports to powerbi;
    GRANT USAGE ON SCHEMA ref to powerbi;

    IF NOT EXISTS (SELECT FROM pg_catalog.pg_user WHERE usename = 'GRP-AzureNhsApp-Developers-DL') THEN
        CREATE USER "GRP-AzureNhsApp-Developers-DL";
    END IF;
    GRANT USAGE ON SCHEMA audit to "GRP-AzureNhsApp-Developers-DL";
    GRANT USAGE ON SCHEMA compute to "GRP-AzureNhsApp-Developers-DL";
    GRANT USAGE ON SCHEMA events to "GRP-AzureNhsApp-Developers-DL";
    GRANT USAGE ON SCHEMA public to "GRP-AzureNhsApp-Developers-DL";
    GRANT USAGE ON SCHEMA reports to "GRP-AzureNhsApp-Developers-DL";
    GRANT USAGE ON SCHEMA ref to "GRP-AzureNhsApp-Developers-DL";

    IF NOT EXISTS (SELECT FROM pg_catalog.pg_user WHERE usename = 'GRP-AzureNhsApp-AnalyticsAdmins-DL') THEN
        CREATE USER "GRP-AzureNhsApp-AnalyticsAdmins-DL";
    END IF;
    GRANT USAGE ON SCHEMA audit to "GRP-AzureNhsApp-AnalyticsAdmins-DL";
    GRANT USAGE ON SCHEMA compute to "GRP-AzureNhsApp-AnalyticsAdmins-DL";
    GRANT USAGE ON SCHEMA events to "GRP-AzureNhsApp-AnalyticsAdmins-DL";
    GRANT USAGE ON SCHEMA public to "GRP-AzureNhsApp-AnalyticsAdmins-DL";
    GRANT USAGE ON SCHEMA reports to "GRP-AzureNhsApp-AnalyticsAdmins-DL";
    GRANT USAGE ON SCHEMA ref to "GRP-AzureNhsApp-AnalyticsAdmins-DL";

    IF NOT EXISTS (SELECT FROM pg_catalog.pg_user WHERE usename = 'AzureNhsAnalytics_DataFactory') THEN
        CREATE USER "AzureNhsAnalytics_DataFactory";
    END IF;
    GRANT USAGE ON SCHEMA compute to "AzureNhsAnalytics_DataFactory";
    GRANT USAGE ON SCHEMA audit to "AzureNhsAnalytics_DataFactory";
END
$$;
