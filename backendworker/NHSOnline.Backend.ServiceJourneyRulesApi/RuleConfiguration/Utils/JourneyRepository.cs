using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    internal class JourneyRepository : IJourneyRepository
    {
        private readonly IDictionary<string, Journeys> _odsJourneys;

        public JourneyRepository(IDictionary<string, Journeys> odsJourneys)
        {
            _odsJourneys = odsJourneys;
        }

        public OdsCodesResponse GetOdsCodes()
        {
            var output = (_odsJourneys?.Keys ?? new List<string>())
                .Where(x => x != Constants.OdsCode.None)
                .ToList();

            return new OdsCodesResponse
            {
                OdsCodes = output
            };
        }

        public Journeys GetJourneys(string odsCode) => _odsJourneys?.ContainsKey(odsCode) ?? false ? _odsJourneys[odsCode] : null;
    }
}