using System.Collections.Generic;
using System.Threading.Tasks;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Service
{
    internal interface IJourneyService
    {
        Task<IDictionary<string, Journeys>> GetJourneys();
    }
}