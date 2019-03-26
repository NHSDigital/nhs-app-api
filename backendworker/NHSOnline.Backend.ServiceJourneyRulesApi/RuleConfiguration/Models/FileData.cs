namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models
{
    public class FileData
    {
        public string Name  { get; set; }
        public string Data  { get; set; }
        public bool IsError { get; set; }
        public string Error { get; set; }
    }
}