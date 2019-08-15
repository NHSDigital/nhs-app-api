namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models
{
    public class ServiceDefinitionItem
    {
        public string Id { get; }
        public string Title { get; }

        public ServiceDefinitionItem(string id, string title)
        {
            this.Id = id;
            this.Title = title;
        }
    }
}