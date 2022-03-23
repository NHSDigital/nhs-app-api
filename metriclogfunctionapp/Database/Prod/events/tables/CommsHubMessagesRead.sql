
CREATE TABLE IF NOT EXISTS events."CommsHubMessagesRead" (
    "Timestamp" timestamp with time zone NOT NULL,
    "MessageId" character varying NOT NULL,
    "ReadTimestamp" timestamp with time zone NOT NULL,
    CONSTRAINT comshubmessagesread_messageid_pk PRIMARY KEY ("MessageId")
);

CALL perms.apply_etl_table_permissions('events', 'CommsHubMessagesRead');

CREATE INDEX IF NOT EXISTS ComsHubMessagesRead_Timestamp_MessageId_ReadTimestamp_idx on events."CommsHubMessagesRead" ("Timestamp", "MessageId", "ReadTimestamp");
