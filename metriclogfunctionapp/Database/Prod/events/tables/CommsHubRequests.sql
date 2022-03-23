
CREATE TABLE IF NOT EXISTS events."CommsHubRequests" (
    "Timestamp" timestamp with time zone NOT NULL,
    "RequestId" character varying NOT NULL,
    "MessageType" character varying NOT NULL,
    "SupplierId" character varying NOT NULL,
    "CampaignRef" character varying NULL,
    "RequestRef" character varying NULL,
    "RecipientType" character varying NOT NULL,
    "ReceivedTimestamp" timestamp with time zone NOT NULL,
    "ProcessedTimestamp" timestamp with time zone NOT NULL,
    CONSTRAINT commshubrequests_requestid_pk PRIMARY KEY ("RequestId")
);

CALL perms.apply_etl_table_permissions('events', 'CommsHubRequests');
