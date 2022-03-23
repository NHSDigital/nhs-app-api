CREATE OR REPLACE PROCEDURE events.CommsHubMessagesSentInsert(
    messageTimestamp timestamp with time zone,
    requestId character varying,
    messagetype character varying,
    loginId character varying,
    messageId character varying,
    processedTimestamp timestamp with time zone,
    status character varying,
    statusDetail character varying
)
AS $$

BEGIN

INSERT INTO "events"."CommsHubMessagesSent" ("Timestamp",
                                             "RequestId",
                                             "Type",
                                             "LoginId",
                                             "MessageId",
                                             "ProcessedTimestamp",
                                             "Status",
                                             "StatusDetail")
VALUES (messageTimestamp,
        requestId,
        messageType,
        loginId,
        messageId,
        processedTimestamp,
        status,
        statusDetail)
    ON CONFLICT ON CONSTRAINT comshubmessagessent_messageid_pk
    DO NOTHING;

END;

$$ LANGUAGE plpgsql;