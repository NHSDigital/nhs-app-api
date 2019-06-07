using System.Threading.Tasks;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;

namespace NHSOnline.Backend.PfsApi.ServiceJourneyRules
{
    public interface IServiceJourneyRulesClient
    {
        Task<ServiceJourneyRulesApiObjectResponse<ServiceJourneyRulesResponse>>
            GetServiceJourneyRules(string odsCode);
    }
}