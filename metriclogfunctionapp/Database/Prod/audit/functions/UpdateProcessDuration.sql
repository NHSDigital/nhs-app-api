DROP FUNCTION IF EXISTS audit.UpdateProcessDuration(int);

CREATE OR REPLACE FUNCTION audit.UpdateProcessDuration(
    "id" int,
    "endDateTime" timestamp with time zone DEFAULT timezone('utc'::text, clock_timestamp())
)
    RETURNS bool
AS $$

DECLARE
    result bool := false;
BEGIN
    UPDATE audit."ProcessDuration"
    SET "EndDateTime" = "endDateTime",
        "Duration" = (ROUND((EXTRACT (EPOCH FROM "endDateTime") - EXTRACT (EPOCH FROM "StartDateTime")) * 1000))
    WHERE "Id" = "id";

    result  = true;
    RETURN result;

END;

$$ LANGUAGE plpgsql;