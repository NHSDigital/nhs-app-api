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
        public string PharmacySearchRadiusKm { get; }

        public GpLookupConfig(IConfiguration configuration, ILogger<GpLookupConfig> logger)
        {
            GpLookupApiResultsLimit = int.Parse(configuration.GetOrWarn("GP_LOOKUP_API_RESULTS_LIMIT", logger), Thread.CurrentThread.CurrentCulture);
            
            PharmacySearchApiLimit =
                int.Parse(configuration.GetOrWarn("PHARMACY_API_RESULT_LIMIT", logger), Thread.CurrentThread.CurrentCulture);
      
            var gpLookupApiUriString = configuration.GetOrWarn("NHS_SEARCH_BASE_URL", logger);
          
            if (!string.IsNullOrEmpty(gpLookupApiUriString))
            {
                NhsSearchBaseUrl = new Uri($"{gpLookupApiUriString}", UriKind.Absolute);
            }

            GpLookupApiKey = configuration.GetOrWarn("GP_LOOKUP_API_KEY", logger);
            
            PostcodeLookupSearchRadiusKm = configuration.GetOrWarn("POSTCODE_LOOKUP_SEARCH_RADIUS_KM", logger);

            PharmacySearchRadiusKm = configuration.GetOrWarn("PHARMACY_SEARCH_RADIUS_KM", logger);
        }
    }
}