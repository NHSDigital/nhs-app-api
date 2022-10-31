CREATE UNIQUE INDEX IF NOT EXISTS CommsHubSummary_pkey ON compute."CommsHubSummary"("SendDate","Supplier","CampaignRef","Type","Status","LoggedIn");
