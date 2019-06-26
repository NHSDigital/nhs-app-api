using NHSOnline.Backend.ServiceJourneyRulesApi.Models;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models
{
    internal class Journeys
    {
        public Appointments Appointments { get; set; }
        
        public Cdss CdssAdvice  { get; set; }
        
        public Cdss CdssAdmin  { get; set; }
    }
}