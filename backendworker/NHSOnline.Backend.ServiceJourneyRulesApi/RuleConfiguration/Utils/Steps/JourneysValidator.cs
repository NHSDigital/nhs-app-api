using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Steps
{
    internal class JourneysValidator
    {
        private readonly IDictionary<Func<Journeys, bool>, string> _journeyValidators =
            new Dictionary<Func<Journeys, bool>, string>();

        public JourneysValidator Add(Func<Journeys, bool> validator, string propertyName)
        {
            _journeyValidators.Add(validator, propertyName);
            return this;
        }

        public IList<string> GetAnyInvalidProperties(Journeys journeys)
        {
            return _journeyValidators
                .Where(validator => !validator.Key.Invoke(journeys))
                .Select(validator => validator.Value).ToList();
        }
    }
}