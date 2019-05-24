using System.Collections.Generic;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    internal class JourneyRepository : IJourneyRepository
    {
        private readonly IDictionary<string, Journeys> _odsJourneys;

        public JourneyRepository(IDictionary<string, Journeys> odsJourneys)
        {
            _odsJourneys = odsJourneys;
        }

        public Journeys GetJourneys(string odsCode) => _odsJourneys[odsCode];
    }
}