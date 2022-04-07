CREATE OR REPLACE PROCEDURE events.CommsHubMessagesLinkClickedInsert(
    messageId character varying,
    link text,
    clickedTimestamp timestamp with time zone,
    eventTimestamp timestamp with time zone
)
AS $$

BEGIN

    INSERT INTO "events"."CommsHubMessagesLinkClicked" ("MessageId",
                                                        "Link",
                                                        "ClickedTimestamp",
                                                        "Timestamp")
    VALUES (messageId,
            link,
            clickedTimestamp,
            eventTimestamp)
    ON CONFLICT ON CONSTRAINT commshubmessageslinkclicked_messageidclickedtimestamp_pk DO NOTHING;

END;

$$ LANGUAGE plpgsql;