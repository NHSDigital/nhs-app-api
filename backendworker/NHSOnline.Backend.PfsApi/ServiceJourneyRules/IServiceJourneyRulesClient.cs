using System.Threading.Tasks;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules.Models;

namespace NHSOnline.Backend.PfsApi.ServiceJourneyRules
{
    public interface IServiceJourneyRulesClient
    {
        Task<ServiceJourneyRulesApiObjectResponse<ServiceJourneyRulesResult>>
            GetServiceJourneyRules(string odsCode);
    }
}