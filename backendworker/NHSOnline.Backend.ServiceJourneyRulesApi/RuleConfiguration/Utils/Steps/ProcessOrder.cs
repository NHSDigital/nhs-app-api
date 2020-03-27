namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Steps
{
    public enum ProcessOrder
    {
        LoadRequiredFiles = 0,
        LoadConfigurationFiles,
        SanitizeOdsJourneys,
        ValidateUniqueOdsConfiguration,
        MergeOdsJourneys,
        ValidateOdsJourneys,
        OutputOdsJourneys,
    }
}