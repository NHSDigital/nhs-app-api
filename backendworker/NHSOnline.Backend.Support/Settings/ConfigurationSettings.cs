using System;
using Microsoft.Extensions.Configuration;

namespace NHSOnline.Backend.Support.Settings
{
    public interface IConfigurationSettings 
    {
        string CookieDomain { get; set; }
        
        int? PrescriptionsDefaultLastNumberMonthsToDisplay { get; set; }

        int DefaultSessionExpiryMinutes { get; set; }

        int DefaultHttpTimeoutSeconds { get; set; }
        
        int MinimumAppAge { get; set; }
        
        int MinimumLinkageAge { get; set; }
        
        DateTimeOffset? CurrentTermsConditionsEffectiveDate { get; set; }  
    }
    
    public class ConfigurationSettings: IConfigurationSettings
    {
        public string CookieDomain { get; set; }
        
        public int? PrescriptionsDefaultLastNumberMonthsToDisplay { get; set; }

        public int DefaultSessionExpiryMinutes { get; set; }

        public int DefaultHttpTimeoutSeconds { get; set; }
        
        public int MinimumAppAge { get; set; }
        
        public int MinimumLinkageAge { get; set; }
        
        public DateTimeOffset? CurrentTermsConditionsEffectiveDate { get; set; }

        public ConfigurationSettings() {}
        
        public ConfigurationSettings(string cookieDomain, int? prescriptionsDefaultLastNumberMonthsToDisplay,
            int defaultSessionExpiryMinutes, int defaultHttpTimeoutSeconds, int minimumAppAge, int minimumLinkageAge, DateTimeOffset? currentTermsConditionsEffectiveDate)
            {
                CookieDomain = cookieDomain;
                PrescriptionsDefaultLastNumberMonthsToDisplay = prescriptionsDefaultLastNumberMonthsToDisplay;
                DefaultHttpTimeoutSeconds = defaultHttpTimeoutSeconds;
                DefaultSessionExpiryMinutes = defaultSessionExpiryMinutes;
                MinimumAppAge = minimumAppAge;
                MinimumLinkageAge = minimumLinkageAge;
                CurrentTermsConditionsEffectiveDate = currentTermsConditionsEffectiveDate;

            }

        public void Validate()
        {
            if (PrescriptionsDefaultLastNumberMonthsToDisplay == null)
            {
                throw new ConfigurationNotFoundException(nameof(PrescriptionsDefaultLastNumberMonthsToDisplay));
            }

            if (DefaultSessionExpiryMinutes == default(int))
            {
                throw new ConfigurationNotFoundException(nameof(DefaultSessionExpiryMinutes));
            }

            if (DefaultHttpTimeoutSeconds == default(int))
            {
                throw new ConfigurationNotFoundException(nameof(DefaultHttpTimeoutSeconds));
            }
            
            if (MinimumAppAge == default(int))
            {
                throw new ConfigurationNotFoundException(nameof(MinimumAppAge));
            }
            
            if (MinimumLinkageAge == default(int))
            {
                throw new ConfigurationNotFoundException(nameof(MinimumLinkageAge));
            }
            
            if (CurrentTermsConditionsEffectiveDate == null)
            {
                throw new ConfigurationNotFoundException(nameof(CurrentTermsConditionsEffectiveDate));
            }

        }
    }
}
