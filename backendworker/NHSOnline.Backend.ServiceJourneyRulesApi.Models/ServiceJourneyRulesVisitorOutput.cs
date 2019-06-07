namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models
{
    public class ServiceJourneyRulesVisitorOutput
    {
        public bool ServiceJourneyRulesRetrieved { get; set; }
        public int StatusCode { get; set; }
        public ServiceJourneyRulesResponse Response { get; set; }
    }
}