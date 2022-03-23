CREATE OR REPLACE PROCEDURE events.CommsHubMessagesReadInsert(
    messageTimestamp timestamp with time zone,
    readTimestamp timestamp with time zone,
    messageId character varying)
AS $$

BEGIN

INSERT INTO "events"."CommsHubMessagesRead" ("Timestamp", "MessageId", "ReadTimestamp")
VALUES ( messageTimestamp, messageId, readTimestamp)
ON CONFLICT ON CONSTRAINT comshubmessagesread_messageid_pk
DO NOTHING;

END;

$$ LANGUAGE plpgsql;