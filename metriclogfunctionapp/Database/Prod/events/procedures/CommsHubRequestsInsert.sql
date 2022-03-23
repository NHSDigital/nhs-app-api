CREATE OR REPLACE PROCEDURE events.CommsHubRequestsInsert(
    messageTimestamp timestamp with time zone,
    requestId character varying,
    messageType character varying,
    supplierId character varying,
    campaignRef character varying,
    requestRef character varying,
    recipientType character varying,
    receivedTimestamp timestamp with time zone,
    processedTimestamp timestamp with time zone
)
AS $$

BEGIN

INSERT INTO "events"."CommsHubRequests" ("Timestamp",
                                             "RequestId",
                                             "MessageType",
                                             "SupplierId",
                                             "CampaignRef",
                                             "RequestRef",
                                             "RecipientType",
                                             "ReceivedTimestamp",
                                             "ProcessedTimestamp")
VALUES (messageTimestamp,
        requestId,
        messageType,
        supplierId,
        campaignRef,
        requestRef,
        recipientType,
        receivedTimestamp,
        processedTimestamp)
    ON CONFLICT ON CONSTRAINT commshubrequests_requestid_pk
    DO NOTHING;

END;

$$ LANGUAGE plpgsql;