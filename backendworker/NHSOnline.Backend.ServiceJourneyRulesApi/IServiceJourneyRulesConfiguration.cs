namespace NHSOnline.Backend.ServiceJourneyRulesApi
{
    internal interface IServiceJourneyRulesConfiguration
    {
        string GpInfoFilePath { get; }

        string OutputFolderPath { get; }

        string RulesFolderPath { get; }

        string JourneysFolderPath { get; }
    }
}