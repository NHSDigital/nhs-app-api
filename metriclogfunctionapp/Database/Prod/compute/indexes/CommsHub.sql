CREATE INDEX IF NOT EXISTS CommsHub_MessageId_idx on "compute"."CommsHub" ("MessageId");

CREATE INDEX IF NOT EXISTS CommsHub_UserFirstLoginTimestamp_idx on "compute"."CommsHub" ("UserFirstLoginTimestamp");

CREATE INDEX IF NOT EXISTS CommsHub_MessageSendTimestamp_idx on "compute"."CommsHub" ("MessageSendTimestamp");

CREATE INDEX IF NOT EXISTS CommsHub_MessageReadTimestamp_idx on "compute"."CommsHub" ("MessageReadTimestamp");