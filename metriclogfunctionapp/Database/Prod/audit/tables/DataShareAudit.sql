CREATE SCHEMA IF NOT EXISTS audit;

CREATE TABLE IF NOT EXISTS audit."DataShareAudit" (
                                                "Id" SERIAL PRIMARY KEY,
                                                "FileName" TEXT,
                                                "DataStartDate" timestamp with time zone,
                                                "DataEndDate" timestamp with time zone,
                                                "ProcessStartDateTime" timestamp with time zone,
                                                "IsSuccess" BOOLEAN,
                                                "ADFPipelineName" TEXT,
                                                "SourceTableName" TEXT
);

CALL perms.apply_compute_table_permissions('audit', 'DataShareAudit');
