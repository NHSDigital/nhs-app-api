using System.Collections.Generic;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Configuration;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Settings
{
    public class OnlineConsultationsProvidersSettings : IValidatable
    {
        public List<OnlineConsultationsProviderSettings> Providers { get; set; }

        public void Validate()
        {
            Providers.ForEach(a => a.Validate());
        }
    }
}