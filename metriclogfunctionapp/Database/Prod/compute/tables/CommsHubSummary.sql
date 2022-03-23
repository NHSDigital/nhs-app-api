CREATE TABLE IF NOT EXISTS compute."CommsHubSummary" (
                                      "SendDate" date,
                                      "Supplier" text,
                                      "CampaignRef" text,
                                      "Type" text,
                                      "Status" text,
                                      "LoggedIn" text,
                                      "AppUsers" int,
                                      "ReadBy" int);

CREATE UNIQUE INDEX IF NOT EXISTS CommsHubSummary_pkey ON compute."CommsHubSummary"("SendDate","Supplier","CampaignRef","Type","Status","LoggedIn");
CALL perms.apply_compute_table_permissions('compute', 'CommsHubSummary');
