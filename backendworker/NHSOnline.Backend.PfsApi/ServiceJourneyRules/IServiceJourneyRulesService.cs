using System.Threading.Tasks;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules.Models;

namespace NHSOnline.Backend.PfsApi.ServiceJourneyRules
{
    public interface IServiceJourneyRulesService
    {
        Task<ServiceJourneyRulesConfigResult> GetServiceJourneyRulesForOdsCode(string odsCode);
        
        Task<ServiceJourneyRulesConfigResult> GetServiceJourneyRulesForLinkedAccount(string odsCode);
    }
}