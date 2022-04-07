CREATE OR REPLACE PROCEDURE audit.InsertDataFactoryProcess(
    "fileName" varchar,
    "dataStartDate" timestamp with time zone,
    "isSuccess" bool,
    "adfPipelineName" varchar,
    "sourceTableName" varchar)
AS $$
BEGIN
    INSERT INTO audit."DataShareAudit" ("FileName","DataStartDate","DataEndDate", "ProcessStartDateTime","IsSuccess", "ADFPipelineName", "SourceTableName")
    VALUES ("fileName","dataStartDate","dataStartDate" +  INTERVAL '1 week', now(),"isSuccess", "adfPipelineName", "sourceTableName");
END;
$$ LANGUAGE plpgsql;

DROP PROCEDURE IF EXISTS
    audit.insertdatafactoryprocess(varchar, timestamp with time zone, boolean);

DROP PROCEDURE IF EXISTS
    audit.insertdatafactoryprocess(varchar, timestamp with time zone, boolean, varchar);
