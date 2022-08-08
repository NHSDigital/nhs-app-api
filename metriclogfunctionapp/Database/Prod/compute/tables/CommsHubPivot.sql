CREATE TABLE IF NOT EXISTS compute."CommsHubPivot" (
                                   "LoginId" text,
                                   "SendDate" date,
                                   "MessageSendTimestamp" timestamp with time zone,
                                   "InAppStatus" text,
                                   "PushStatus" text,
                                   "Supplier" text,
                                   "CampaignRef" text,
                                   "RequestId" text,
                                   "MessageReadTimestamp" timestamp with time zone,
                                   "InAppUserFirstLoginTimestamp" timestamp with time zone,
                                   "PushUserFirstLoginTimestamp" timestamp with time zone,
                                   primary key("LoginId","RequestId"));

ALTER TABLE compute."CommsHubPivot"
            ADD COLUMN IF NOT EXISTS "NotificationOutcomeStatus" character varying(36) NULL,
            ADD COLUMN IF NOT EXISTS "Links" int NULL,
            ADD COLUMN IF NOT EXISTS "DistinctLinks" int NULL,
            ADD COLUMN IF NOT EXISTS "AnyLink" int NULL,
            ADD COLUMN IF NOT EXISTS "FirstLinkClickedTimestamp" timestamp with time zone NULL,
            ADD COLUMN IF NOT EXISTS "ReceivedTimestamp" timestamp with time zone NULL,
            ADD COLUMN IF NOT EXISTS "EndDateTime" timestamp with time zone NULL;

CALL perms.apply_compute_table_permissions('compute', 'CommsHubPivot');