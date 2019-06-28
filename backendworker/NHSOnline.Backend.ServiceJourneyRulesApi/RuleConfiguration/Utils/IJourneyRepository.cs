using NHSOnline.Backend.ServiceJourneyRulesApi.Models;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    internal interface IJourneyRepository
    {
        Journeys GetJourneys(string odsCode);
    }
}