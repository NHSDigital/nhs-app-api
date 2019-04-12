using System;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public interface INominatedPharmacyConfig
    {
        Uri BaseUrl { get; }

        string SpineAccreditedSystemIdFrom { get; }

        string SpineAccreditedSystemIdTo { get; }

        string PdsQueryTo { get; }

        string PdsQueryFromAddress { get; }

        string PartSdsRoleId { get; }

        string SdsUserId { get; }

        string PersonSdsRoleId { get; }
        
        string MessageId { get; }
    }
}
