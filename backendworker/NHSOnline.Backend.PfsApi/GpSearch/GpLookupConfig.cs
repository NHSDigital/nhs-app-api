using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.GpSearch
{
    public class GpLookupConfig: IGpLookupConfig
    {
        public int PharmacySearchApiLimit { get; }
        public int OnlinePharmacyRandomisedSearchResultLimit { get; }
        public int OnlinePharmacySearchResultLimit { get; }
        public Uri NhsSearchBaseUrl { get; }
        public string GpLookupApiKey { get; }
        public string GpPracticeOdsCodeForEpsEnabledCheckOverride { get; }

        public GpLookupConfig(IConfiguration configuration, ILogger<GpLookupConfig> logger)
        {
            PharmacySearchApiLimit = configuration.GetIntOrThrow("PHARMACY_API_RESULT_LIMIT", logger);

            OnlinePharmacyRandomisedSearchResultLimit = configuration.GetIntOrThrow("ONLINE_PHARMACY_RANDOMISED_SEARCH_RESULT_LIMIT", logger);
            
            OnlinePharmacySearchResultLimit = configuration.GetIntOrThrow("ONLINE_PHARMACY_SEARCH_RESULT_LIMIT", logger);

            var gpLookupApiUriString = configuration.GetOrWarn("NHS_SEARCH_BASE_URL", logger);

            if (!string.IsNullOrEmpty(gpLookupApiUriString))
            {
                NhsSearchBaseUrl = new Uri($"{gpLookupApiUriString}", UriKind.Absolute);
            }

            GpLookupApiKey = configuration.GetOrWarn("GP_LOOKUP_API_KEY", logger);

            GpPracticeOdsCodeForEpsEnabledCheckOverride = configuration.GetOrWarn("GP_PRACTICE_ODS_CODE_FOR_EPS_ENABLED_CHECK_OVERRIDE", logger);
        }
    }
}
