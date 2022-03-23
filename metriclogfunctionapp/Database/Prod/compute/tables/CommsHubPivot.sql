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

CALL perms.apply_compute_table_permissions('compute', 'CommsHubPivot');