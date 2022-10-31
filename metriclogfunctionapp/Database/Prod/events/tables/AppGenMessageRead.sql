CREATE TABLE IF NOT EXISTS events."AppGenMessageRead" (
                                                          "MessageId" character varying(36) NOT NULL,
                                                          "CampaignId" character varying(36) NOT NULL,
                                                          "LoginId" character varying(36) NOT NULL,
                                                          "Timestamp" timestamp with time zone NOT NULL
);

CALL perms.apply_etl_table_permissions('events', 'AppGenMessageRead');