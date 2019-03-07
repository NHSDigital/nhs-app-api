using Newtonsoft.Json;

namespace NHSOnline.Backend.PfsApi.ServiceJourneyRules.Models
{
    public class ServiceJourneyRulesResult : IServiceJourneyResult
    {
        public ServiceJourneyRulesAppointments Appointments { get; set; }
    }
}