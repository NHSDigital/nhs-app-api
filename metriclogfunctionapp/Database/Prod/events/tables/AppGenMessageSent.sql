CREATE TABLE IF NOT EXISTS events."AppGenMessageSent" (
                                                          "MessageId" character varying(36) NOT NULL,
                                                          "CampaignId" character varying(36) NOT NULL,
                                                          "LoginId" character varying(36) NOT NULL,
                                                          "Timestamp" timestamp with time zone NOT NULL
);

CALL perms.apply_etl_table_permissions('events', 'AppGenMessageSent');

CREATE INDEX IF NOT EXISTS AppGenMessageSent_MessageId_idx on events."AppGenMessageSent" ("MessageId");