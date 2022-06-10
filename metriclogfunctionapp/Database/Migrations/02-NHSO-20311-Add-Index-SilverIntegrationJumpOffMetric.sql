CREATE INDEX IF NOT EXISTS SilverIntegrationJumpOffMetric_SessionId_idx
ON events."SilverIntegrationJumpOffMetric" ("SessionId");

CREATE INDEX IF NOT EXISTS SilverIntegrationJumpOffMetric_ProviderName_idx 
ON events."SilverIntegrationJumpOffMetric" ("ProviderName");

CREATE INDEX IF NOT EXISTS SilverIntegrationJumpOffMetric_SessionId_CovidJumpOff_idx 
ON events."SilverIntegrationJumpOffMetric" ("SessionId")
WHERE "ProviderName" = 'the Department of Health and Social Care';

CREATE INDEX IF NOT EXISTS SilverIntegrationJumpOffMetric_SessionId_OtherJumpOff_idx 
ON events."SilverIntegrationJumpOffMetric" ("SessionId")
WHERE "ProviderName" <> 'the Department of Health and Social Care';