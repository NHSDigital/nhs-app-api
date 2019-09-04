using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models
{
    internal class GpInfo
    {
        public string Ods { get; set; }

        public GpInfoSupplier Supplier { get; set; }

        public string EndpointCreated { get; set; }
        
        public string CcgCode { get; set; }
    }
}