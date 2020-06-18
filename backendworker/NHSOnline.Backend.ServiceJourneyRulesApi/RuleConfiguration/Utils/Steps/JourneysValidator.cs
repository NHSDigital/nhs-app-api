using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Steps
{
    internal sealed class JourneysValidator
    {
        private readonly List<(Func<string, Journeys, bool> validator, string propertyName)> _journeyValidators
            = new List<(Func<string, Journeys, bool> validator, string propertyName)>();

        public JourneysValidator Add(Func<Journeys, bool> validator, string propertyName)
            => Add((_, journeys) => validator.Invoke(journeys), propertyName);

        public JourneysValidator Add(Func<string, Journeys, bool> validator, string propertyName)
        {
            _journeyValidators.Add((validator, propertyName));
            return this;
        }

        public IList<string> GetAnyInvalidProperties(string odsCode, Journeys journeys)
        {
            return _journeyValidators
                .Where(validator => !validator.validator(odsCode, journeys))
                .Select(validator => validator.propertyName).ToList();
        }
    }
}