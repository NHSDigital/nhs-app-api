using System.Collections.Generic;
using System.Threading.Tasks;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Service
{
    internal interface IJourneyService
    {
        Task<IDictionary<string, Journeys>> GetJourneys();
    }
}