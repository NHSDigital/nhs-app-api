namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models
{
    internal class FileData
    {

        public string Name { get; }
        
        public string Data { get; }
        
        public FileData(string name, string data)
        {
            Name = name;
            Data = data;
        }
    }
}