namespace NHSOnline.Backend.ServiceJourneyRulesApi
{
    internal static class Constants
    {
        public static class Args
        {
            public const string ValidateMode = "--validate-only";
        }

        public static class FileNames
        {
            public const string RulesSchema = "rules_schema.json";
            public const string JourneyConfigurationSchema = "configuration_schema.json";
        }

        public static class Target
        {
            public const string All = "*";
        }
    }
}