using System.Threading.Tasks;
using NHSOnline.Backend.ServiceJourneyRules.Common.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;

namespace NHSOnline.Backend.ServiceJourneyRules.Common
{
    public interface IServiceJourneyRulesClient
    {
        Task<ServiceJourneyRulesApiObjectResponse<ServiceJourneyRulesResponse>>
            GetServiceJourneyRules(string odsCode);
    }
}