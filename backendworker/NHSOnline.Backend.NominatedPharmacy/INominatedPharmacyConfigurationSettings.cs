using System;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public interface INominatedPharmacyConfigurationSettings
    {
        bool IsNominatedPharmacyEnabled { get; }

        Uri BaseUrl { get; }

        string SpineAccreditedSystemIdFrom { get; }

        string SpineAccreditedSystemIdTo { get; }

        string PdsQueryTo { get; }

        string PdsQueryFromAddress { get; }

        int ArtificialDelayAfterNominatedPharmacyUpdateInMilliseconds { get; }
        
        string PartyIdFrom { get; }
        
        string PartyIdTo { get; }

    }
}
