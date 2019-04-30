using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Service
{
    public class ServiceJourneyRulesService: IServiceJourneyRulesService
    {
        
        public ServiceJourneyRulesResponse GetServiceJourneyRulesForOdsCode(string odsCode)
        {
            return new ServiceJourneyRulesResponse
            {
                Appointments = new Appointments
                {
                    JourneyType = AppointmentsJourneyType.Im1Appointments
                }
            };
        }
    }
}
