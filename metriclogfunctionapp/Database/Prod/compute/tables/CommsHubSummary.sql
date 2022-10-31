CREATE TABLE IF NOT EXISTS compute."CommsHubSummary" (
                                      "SendDate" date,
                                      "Supplier" text,
                                      "CampaignRef" text,
                                      "Type" text,
                                      "Status" text,
                                      "LoggedIn" text,
                                      "AppUsers" int,
                                      "ReadBy" int);

CALL perms.apply_compute_table_permissions('compute', 'CommsHubSummary');
