using System;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Service
{
    public class ServiceJourneyRulesService: IServiceJourneyRulesService
    {
        private const string AppointmentsJourney = "im1Appointments";
        
        public ServiceJourneyRulesResponse GetServiceJourneyRulesForOdsCode(string odsCode)
        {
            return new ServiceJourneyRulesResponse
            {
                Appointments = new Appointments
                {
                    JourneyType = AppointmentsJourney
                }
            };
        }
    }
}
