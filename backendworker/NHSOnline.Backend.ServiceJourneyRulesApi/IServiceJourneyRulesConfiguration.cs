namespace NHSOnline.Backend.ServiceJourneyRulesApi
{
    internal interface IServiceJourneyRulesConfiguration
    {
        string GpInfoFilePath { get; }
        
        string RulesFolderPath { get; }
        
        string JourneysFolderPath { get; }
        
    }
}