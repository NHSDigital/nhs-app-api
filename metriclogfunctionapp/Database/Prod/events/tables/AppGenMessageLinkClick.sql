CREATE TABLE IF NOT EXISTS events."AppGenMessageLinkClick" (
                                                               "MessageId" character varying(36) NOT NULL,
                                                               "CampaignId" character varying(36) NOT NULL,
                                                               "LoginId" character varying(36) NOT NULL,
                                                               "Link" text NOT NULL,
                                                               "Timestamp" timestamp with time zone NOT NULL
);

CALL perms.apply_etl_table_permissions('events', 'AppGenMessageLinkClick');