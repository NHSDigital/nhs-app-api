CREATE OR REPLACE PROCEDURE audit.InsertDataFactoryProcess(
    "fileName" varchar,
    "dataStartDate" timestamp with time zone,
    "isSuccess" bool,
    "adfPipelineName" varchar)
AS $$
BEGIN
    INSERT INTO audit."DataShareAudit" ("FileName","DataStartDate","DataEndDate", "ProcessStartDateTime","IsSuccess", "ADFPipelineName")
    VALUES ("fileName","dataStartDate","dataStartDate" +  INTERVAL '1 week', now(),"isSuccess", "adfPipelineName");
END;
$$ LANGUAGE plpgsql;