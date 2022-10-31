CREATE INDEX IF NOT EXISTS GeneralPracticeSnapshot_Timestamp_OdsCode_CCGCode_Name_Stat_idx on events."GeneralPracticeSnapshot" ("Timestamp", "OdsCode", "CCGCode", "Name", "Status");
