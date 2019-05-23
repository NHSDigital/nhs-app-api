using System.Collections.Generic;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Settings
{
    public class OnlineConsultationsProvidersSettings : IValidatable
    {
        public List<OnlineConsultationsProviderSettings> Providers { get; set; }
        
        public void Validate()
        {
            foreach (var onlineConsultationsProviderSetting in Providers)
            {
                onlineConsultationsProviderSetting.Validate();
            }
        }
    }
}