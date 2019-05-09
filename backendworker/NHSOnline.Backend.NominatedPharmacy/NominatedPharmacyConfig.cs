using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public class NominatedPharmacyConfig : INominatedPharmacyConfig
    {
        public NominatedPharmacyConfig(IConfiguration configuration, ILogger<NominatedPharmacyConfig> logger)
        {
            IsNominatedPharmacyEnabled = bool.TrueString.Equals(configuration.GetOrWarn("NOMINATED_PHARMACY_ENABLED", logger), StringComparison.OrdinalIgnoreCase);
            SpineAccreditedSystemIdFrom = configuration.GetOrWarn("SPINE_ACCREDITED_SYSTEM_ID_FROM", logger);
            SpineAccreditedSystemIdTo = configuration.GetOrWarn("SPINE_ACCREDITED_SYSTEM_ID_TO", logger);
            PdsQueryFromAddress = configuration.GetOrWarn("PDS_QUERY_FROM_ADDRESS", logger);
            PdsQueryTo = configuration.GetOrWarn("PDS_QUERY_TO", logger);
            PartSdsRoleId = configuration.GetOrWarn("PART_SDS_ROLE_ID", logger);
            SdsUserId = configuration.GetOrWarn("SDS_USER_ID", logger);
            PersonSdsRoleId = configuration.GetOrWarn("PERSON_SDS_ROLE_ID", logger);
            ArtificialDelayAfterNominatedPharmacyUpdateInMilliseconds = configuration.GetIntOrDefault("DELAY_AFTER_NOMINATED_PHARMACY_UPDATE_IN_MILLISECONDS", logger);
            MessageId = Guid.NewGuid().ToString();
            
            var nominatedPharmacyUriString = configuration.GetOrWarn("NOMINATED_PHARMACY_URL", logger);

            if (!string.IsNullOrEmpty(nominatedPharmacyUriString))
            {
                BaseUrl = new Uri(nominatedPharmacyUriString, UriKind.Absolute);
            }
        }

        public Uri BaseUrl { get; }

        public string SpineAccreditedSystemIdFrom { get; }

        public string SpineAccreditedSystemIdTo { get; }

        public string PdsQueryFromAddress { get; }
        
        public string PdsQueryTo { get; }
        
        public string PartSdsRoleId { get; }

        public string SdsUserId { get; }

        public string PersonSdsRoleId { get; }

        public int ArtificialDelayAfterNominatedPharmacyUpdateInMilliseconds { get; }

        public string MessageId { get; }

        public bool IsNominatedPharmacyEnabled { get; }
    }
}
