using System;
using NHSOnline.Backend.Support.Configuration;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Settings
{
    public class OnlineConsultationsProviderSettings : IValidatable
    {
        public string Provider { get; set; }
        public string BaseAddress { get; set; }
        public string ProviderName { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(BaseAddress))
            {
                throw new ConfigurationNotFoundException("OnlineConsultationsProvider.BaseAddress ");
            }

            if (string.IsNullOrWhiteSpace(Provider))
            {
                throw new ConfigurationNotFoundException("OnlineConsultationsProvider.Provider");
            }
            
            if (string.IsNullOrWhiteSpace(ProviderName))
            {
                throw new ConfigurationNotFoundException("OnlineConsultationsProvider.ProviderName");
            }

            var uri = new Uri(BaseAddress);
        }
    }
}