using NHSOnline.Backend.ServiceJourneyRulesApi.Models;

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
                    Provider = AppointmentsProvider.im1
                }
            };
        }
    }
}
