using System.Threading.Tasks;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    internal interface IConfigurationRuleFileValidator
    {
        Task<int> ValidateJourneyConfigurationFiles();
    }
}