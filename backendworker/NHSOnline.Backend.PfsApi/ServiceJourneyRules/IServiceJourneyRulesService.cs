using System.Threading.Tasks;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules.Models;

namespace NHSOnline.Backend.PfsApi.ServiceJourneyRules
{
    public interface IServiceJourneyRulesService
    {
        Task<bool> IsJourneyEnabled(string odsCode);
        
        Task<ServiceJourneyRulesConfigResult> GetServiceJourneyRulesForOdsCode(string odsCode);
    }
}