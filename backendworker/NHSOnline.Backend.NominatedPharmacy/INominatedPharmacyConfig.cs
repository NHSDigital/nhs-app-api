using System;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public interface INominatedPharmacyConfig
    {
        Uri BaseUrl { get; }

        string SpineAccreditedSystemIdFrom { get; }

        string SpineAccreditedSystemIdTo { get; }
        
        string SpineIp { get; }

        string SdsRole { get; }

        string SdsUserId { get; }

        string SdsRoleId { get; }
        
        string MessageId { get; }
    }
}
