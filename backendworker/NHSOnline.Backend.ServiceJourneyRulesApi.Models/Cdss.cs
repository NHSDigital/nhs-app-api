namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models
{
    public class Cdss : Journey<CdssProvider>
    {
        public string ServiceDefinition { get; set; }
    }
}