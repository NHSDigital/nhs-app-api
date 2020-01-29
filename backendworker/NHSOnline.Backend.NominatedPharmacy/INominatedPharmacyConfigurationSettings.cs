using System;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public interface INominatedPharmacyConfigurationSettings
    {
        event EventHandler<NominatedPharmacyConfigurationUpdatedEventArgs> SettingsUpdated;

        bool IsNominatedPharmacyEnabled { get; set; }

        Uri BaseUrl { get; }

        string PdsPath { get; }

        int ArtificialDelayAfterNominatedPharmacyUpdateInMilliseconds { get; }

        PdsTraceConfigurationSettings PdsTraceConfigurationSettings { get; }

        PdsUpdateConfigurationSettings PdsUpdateConfigurationSettings { get; }

        void Update(
            bool isNominatedPharmacyEnabled,
            Uri baseUrl,
            string pdsPath,
            int artificialDelayAfterNominatedPharmacyUpdateInMilliseconds,
            PdsTraceConfigurationSettings pdsTraceConfigurationSettings,
            PdsUpdateConfigurationSettings pdsUpdateConfigurationSettings
        );

        bool Validate();
    }
}
