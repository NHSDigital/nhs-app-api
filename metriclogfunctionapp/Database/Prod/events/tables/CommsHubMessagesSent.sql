
CREATE TABLE IF NOT EXISTS events."CommsHubMessagesSent" (
    "Timestamp" timestamp with time zone NOT NULL,
    "RequestId" character varying NOT NULL,
    "Type" character varying NOT NULL,
    "LoginId" character varying NULL,
    "MessageId" character varying NOT NULL,
    "ProcessedTimestamp" timestamp with time zone NULL,
    "Status" character varying NOT NULL,
    "StatusDetail" character varying NULL,
    CONSTRAINT comshubmessagessent_messageid_pk PRIMARY KEY ("MessageId")
);

CALL perms.apply_etl_table_permissions('events', 'CommsHubMessagesSent');
