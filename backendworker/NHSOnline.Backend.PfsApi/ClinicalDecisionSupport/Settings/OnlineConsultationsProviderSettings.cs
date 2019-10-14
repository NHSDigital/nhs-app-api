using System;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Settings
{
    public class OnlineConsultationsProviderSettings : IValidatable
    {
        public string Provider { get; set; }
        public string BaseAddress { get; set; }
        public string BearerToken { get; set; }
        
        public string ProviderName { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Provider))
            {
                throw new ConfigurationNotFoundException("OnlineConsultationsProvider.Provider");
            }
            
            if (string.IsNullOrWhiteSpace(ProviderName))
            {
                throw new ConfigurationNotFoundException("OnlineConsultationsProvider.ProviderName");
            }
            
            if (string.IsNullOrWhiteSpace(BearerToken))
            {
                throw new ConfigurationNotFoundException("OnlineConsultationsProvider.BearerToken");
            }

            var uri = new Uri(BaseAddress);
        }
    }
}     