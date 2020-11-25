using System.Threading.Tasks;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    internal interface IConfigurationExporter
    {
        Task<int> Export();
    }
}
