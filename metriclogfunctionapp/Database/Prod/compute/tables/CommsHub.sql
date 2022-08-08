CREATE TABLE IF NOT EXISTS compute."CommsHub" (
                                                  "MessageId" character varying(36) NOT NULL UNIQUE,
                                                  "LoginId" character varying(36),
                                                  "SendDate" date,
                                                  "Type" character varying (7),
                                                  "Status" character varying (9),
                                                  "Supplier" character varying (36),
                                                  "CampaignRef" character varying(50),
                                                  "ReadEvents" int,
                                                  "LoggedIn" int,
                                                  "MessageReadTimestamp" timestamp with time zone,
                                                  "MessageSendTimestamp" timestamp with time zone,
                                                  "UserFirstLoginTimestamp" timestamp with time zone,
                                                  "RequestId" character varying(50)
);

ALTER TABLE compute."CommsHub"
            ADD COLUMN IF NOT EXISTS "NotificationOutcomeStatus" character varying(36) NULL,
            ADD COLUMN IF NOT EXISTS "Links" int NULL,
            ADD COLUMN IF NOT EXISTS "DistinctLinks" int NULL,
            ADD COLUMN IF NOT EXISTS "AnyLink" int NULL,
            ADD COLUMN IF NOT EXISTS "FirstLinkClickedTimestamp" timestamp with time zone NULL,
            ADD COLUMN IF NOT EXISTS "ReceivedTimestamp" timestamp with time zone NULL,
            ADD COLUMN IF NOT EXISTS "EndDateTime" timestamp with time zone NULL;

CREATE INDEX IF NOT EXISTS CommsHub_MessageId_idx on "compute"."CommsHub" ("MessageId");

CREATE INDEX IF NOT EXISTS CommsHub_UserFirstLoginTimestamp_idx on "compute"."CommsHub" ("UserFirstLoginTimestamp");

CREATE INDEX IF NOT EXISTS CommsHub_MessageSendTimestamp_idx on "compute"."CommsHub" ("MessageSendTimestamp");

CREATE INDEX IF NOT EXISTS CommsHub_MessageReadTimestamp_idx on "compute"."CommsHub" ("MessageReadTimestamp");

CALL perms.apply_compute_table_permissions('compute', 'CommsHub');