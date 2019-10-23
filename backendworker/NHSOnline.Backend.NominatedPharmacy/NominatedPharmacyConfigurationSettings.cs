using System;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public class NominatedPharmacyConfigurationSettings : INominatedPharmacyConfigurationSettings
    {
        public NominatedPharmacyConfigurationSettings(
            bool isNominatedPharmacyEnabled,
            Uri baseUrl,
            string pdsPath,
            int artificialDelayAfterNominatedPharmacyUpdateInMilliseconds,
            PdsTraceConfigurationSettings pdsTraceConfigurationSettings,
            PdsUpdateConfigurationSettings pdsUpdateConfigurationSettings
            )
        {
            IsNominatedPharmacyEnabled = isNominatedPharmacyEnabled;
            BaseUrl = baseUrl;
            PdsPath = pdsPath;
            ArtificialDelayAfterNominatedPharmacyUpdateInMilliseconds = artificialDelayAfterNominatedPharmacyUpdateInMilliseconds;
            PdsTraceConfigurationSettings = pdsTraceConfigurationSettings;
            PdsUpdateConfigurationSettings = pdsUpdateConfigurationSettings;
        }

        public bool IsNominatedPharmacyEnabled { get; set; }

        public Uri BaseUrl { get; }
        
        public string PdsPath { get; }

        public int ArtificialDelayAfterNominatedPharmacyUpdateInMilliseconds { get; }

        public PdsTraceConfigurationSettings PdsTraceConfigurationSettings { get; }

        public PdsUpdateConfigurationSettings PdsUpdateConfigurationSettings { get; }

        public bool Validate()
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

            return PdsTraceConfigurationSettings.Validate() && PdsUpdateConfigurationSettings.Validate();
        }
    }
}
