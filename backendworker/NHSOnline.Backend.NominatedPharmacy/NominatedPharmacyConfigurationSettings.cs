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

        public event EventHandler<NominatedPharmacyConfigurationUpdatedEventArgs> SettingsUpdated;

        private void OnSettingsUpdated()
        {
            var handler = SettingsUpdated;
            handler?.Invoke(this, new NominatedPharmacyConfigurationUpdatedEventArgs { Config = this});
        }

        public void Update(
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

            OnSettingsUpdated();
        }

        public bool IsNominatedPharmacyEnabled { get; set; }

        public Uri BaseUrl { get; private set; }

        public string PdsPath { get; private set; }

        public int ArtificialDelayAfterNominatedPharmacyUpdateInMilliseconds { get; private set; }

        public PdsTraceConfigurationSettings PdsTraceConfigurationSettings { get; private set; }

        public PdsUpdateConfigurationSettings PdsUpdateConfigurationSettings { get; private set; }

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
