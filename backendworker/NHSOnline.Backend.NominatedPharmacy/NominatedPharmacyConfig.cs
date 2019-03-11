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
            SpineAccreditedSystemIdFrom = configuration.GetOrWarn("SPINE_ACCREDITED_SYSTEM_ID_FROM", logger);
            SpineAccreditedSystemIdTo = configuration.GetOrWarn("SPINE_ACCREDITED_SYSTEM_ID_TO", logger);
            SpineIp = configuration.GetOrWarn("SPINE_IP", logger);
            SdsRole = configuration.GetOrWarn("SDS_ROLE_ID", logger);
            SdsUserId = configuration.GetOrWarn("SDS_USER_ID", logger);
            SdsRoleId = configuration.GetOrWarn("SDS_ROLE_ID", logger);
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

        public string SpineIp { get; }

        public string SdsRole { get; }

        public string SdsUserId { get; }

        public string SdsRoleId { get; }

        public string MessageId { get;  }
    }
}
