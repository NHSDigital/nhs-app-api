using System;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public interface INominatedPharmacyConfigurationSettings
    {
        bool IsNominatedPharmacyEnabled { get; }

        Uri BaseUrl { get; }

        int ArtificialDelayAfterNominatedPharmacyUpdateInMilliseconds { get; }

        PdsTraceConfigurationSettings PdsTraceConfigurationSettings { get; }

        PdsUpdateConfigurationSettings PdsUpdateConfigurationSettings { get; }
    }
}
