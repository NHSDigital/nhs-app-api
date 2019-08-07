using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.GpSearch
{
    public class GpLookupConfig: IGpLookupConfig
    {
        public int GpLookupApiResultsLimit { get; }
        public int PharmacySearchApiLimit { get; }
        public Uri NhsSearchBaseUrl { get; }
        public string GpLookupApiKey { get; }
        public string PostcodeLookupSearchRadiusKm { get; }
        public string GpPraticeOdsCodeForEpsEnabledCheckOverride { get; }

        public GpLookupConfig(IConfiguration configuration, ILogger<GpLookupConfig> logger)
        {
            GpLookupApiResultsLimit = configuration.GetIntOrThrow("GP_LOOKUP_API_RESULTS_LIMIT", logger);
            
            PharmacySearchApiLimit = configuration.GetIntOrThrow("PHARMACY_API_RESULT_LIMIT", logger);
      
            var gpLookupApiUriString = configuration.GetOrWarn("NHS_SEARCH_BASE_URL", logger);
          
            if (!string.IsNullOrEmpty(gpLookupApiUriString))
            {
                NhsSearchBaseUrl = new Uri($"{gpLookupApiUriString}", UriKind.Absolute);
            }

            GpLookupApiKey = configuration.GetOrWarn("GP_LOOKUP_API_KEY", logger);
            
            PostcodeLookupSearchRadiusKm = configuration.GetOrWarn("POSTCODE_LOOKUP_SEARCH_RADIUS_KM", logger);

            GpPraticeOdsCodeForEpsEnabledCheckOverride = configuration.GetOrWarn("GP_PRACTICE_ODS_CODE_FOR_EPS_ENABLED_CHECK_OVERRIDE", logger);
        }
    }
}
