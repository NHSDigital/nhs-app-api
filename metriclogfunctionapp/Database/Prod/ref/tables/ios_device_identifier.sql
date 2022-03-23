CREATE TABLE IF NOT EXISTS "ref"."ios_device_identifier" (
               "Generation" varchar COLLATE "default",
               "A Number" varchar COLLATE "default",
               "Bootrom" varchar COLLATE "default",
               "FCC ID" varchar COLLATE "default",
               "Internal Name" varchar COLLATE "default",
               "Identifier" varchar COLLATE "default",
               "Finish" varchar COLLATE "default",
               "Storage" varchar COLLATE "default",
               "Model" varchar COLLATE "default"
);

CALL perms.apply_etl_select_permissions('ref', 'ios_device_identifier');