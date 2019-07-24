using System;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public class NominatedPharmacyConfigurationSettings : INominatedPharmacyConfigurationSettings
    {
        public NominatedPharmacyConfigurationSettings(
            bool isNominatedPharmacyEnabled,
            Uri baseUrl,
            int artificialDelayAfterNominatedPharmacyUpdateInMilliseconds,
            PdsTraceConfigurationSettings pdsTraceConfigurationSettings,
            PdsUpdateConfigurationSettings pdsUpdateConfigurationSettings
            )
        {
            IsNominatedPharmacyEnabled = isNominatedPharmacyEnabled;
            BaseUrl = baseUrl;
            ArtificialDelayAfterNominatedPharmacyUpdateInMilliseconds = artificialDelayAfterNominatedPharmacyUpdateInMilliseconds;
            PdsTraceConfigurationSettings = pdsTraceConfigurationSettings;
            PdsUpdateConfigurationSettings = pdsUpdateConfigurationSettings;
        }

        public bool IsNominatedPharmacyEnabled { get; }

        public Uri BaseUrl { get; }
        
        public int ArtificialDelayAfterNominatedPharmacyUpdateInMilliseconds { get; }

        public PdsTraceConfigurationSettings PdsTraceConfigurationSettings { get; }

        public PdsUpdateConfigurationSettings PdsUpdateConfigurationSettings { get; }

        public void Validate()
        {
            if (BaseUrl == null)
            {
                throw new ConfigurationNotFoundException(nameof(BaseUrl));
            }

            if (PdsTraceConfigurationSettings == null)
            {
                throw new ConfigurationNotFoundException(nameof(PdsTraceConfigurationSettings));
            }
            
            if (PdsUpdateConfigurationSettings == null)
            {
                throw new ConfigurationNotFoundException(nameof(PdsUpdateConfigurationSettings));
            }

            PdsTraceConfigurationSettings.Validate();
            PdsUpdateConfigurationSettings.Validate();
        }
    }
}
