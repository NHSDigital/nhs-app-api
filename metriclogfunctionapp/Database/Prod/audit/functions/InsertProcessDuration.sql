DROP FUNCTION IF EXISTS audit.InsertProcessDuration(varchar,timestamp with time zone);

CREATE OR REPLACE FUNCTION audit.InsertProcessDuration(
    "processName" varchar,
    "startDateTime" timestamp with time zone DEFAULT timezone('utc'::text, clock_timestamp()))
    RETURNS int
AS $$

DECLARE
    newId int;
BEGIN
    INSERT INTO audit."ProcessDuration" ("ProcessName", "StartDateTime")
    VALUES ("processName", "startDateTime")
    RETURNING "Id" INTO newId;

    RETURN newId;
END;

$$ LANGUAGE plpgsql;