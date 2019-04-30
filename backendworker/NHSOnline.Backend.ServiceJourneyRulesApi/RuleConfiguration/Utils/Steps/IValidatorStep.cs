using System.Threading.Tasks;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Steps
{
    internal interface IValidatorStep
    {
        
        string Description { get; }
        
        ProcessOrder Order { get; }

        Task<bool> Execute(ConfigurationContext context);

    }
}