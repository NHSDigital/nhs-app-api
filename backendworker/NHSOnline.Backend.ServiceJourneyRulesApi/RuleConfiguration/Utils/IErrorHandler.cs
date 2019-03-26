namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    public interface IErrorHandler
    {
        void LogCritical(string message);
        void LogError(string message);
        void LogInformation(string message);
    }
}