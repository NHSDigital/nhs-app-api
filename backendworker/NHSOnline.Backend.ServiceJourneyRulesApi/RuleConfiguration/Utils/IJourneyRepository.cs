using NHSOnline.Backend.ServiceJourneyRulesApi.Models;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    public interface IJourneyRepository
    {
        OdsCodesResponse GetOdsCodes();
        Journeys GetJourneys(string odsCode);
    }
}