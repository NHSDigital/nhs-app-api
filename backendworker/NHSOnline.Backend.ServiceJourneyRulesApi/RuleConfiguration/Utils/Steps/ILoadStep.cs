using System.Threading.Tasks;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Steps
{
    internal interface ILoadStep
    {
        string Description { get; }
        
        ProcessOrder Order { get; }

        Task<bool> Execute(LoadContext context);
    }
}