namespace NHSOnline.Backend.ServiceJourneyRulesApi
{
    public static class Constants
    {
        public static class Args
        {
            public const string ValidateMode = "--validate-only";
        }

        public static class FileNames
        {
            public const string RulesConfiguration = "rules.yaml";
            public const string RulesSchema = "rules_schema.json";
            public const string JourneyConfigurationSchema = "configuration_schema.json";
        }

        public static class FolderNames
        {
            public const string JourneyConfigurations = "Configurations/Journeys";
            public const string RulesConfiguration = "Configurations/Rules";
        }
    }
}