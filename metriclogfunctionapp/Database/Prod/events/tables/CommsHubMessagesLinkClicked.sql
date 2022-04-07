
CREATE TABLE IF NOT EXISTS events."CommsHubMessagesLinkClicked" (
        "MessageId" character varying NOT NULL,
        "Link" text NOT NULL,
        "ClickedTimestamp" timestamp with time zone NOT NULL,
        "Timestamp" timestamp with time zone NOT NULL,
        CONSTRAINT commshubmessageslinkclicked_messageidclickedtimestamp_pk PRIMARY KEY ("MessageId", "ClickedTimestamp")
);

CALL perms.apply_etl_table_permissions('events', 'CommsHubMessagesLinkClicked');