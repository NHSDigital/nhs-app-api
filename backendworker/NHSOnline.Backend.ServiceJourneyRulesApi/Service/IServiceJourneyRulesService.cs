using NHSOnline.Backend.ServiceJourneyRulesApi.Models;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Service
{
    public interface IServiceJourneyRulesService
    {
        ServiceJourneyRulesResponse GetServiceJourneyRulesForOdsCode(string odsCode);
    }
}