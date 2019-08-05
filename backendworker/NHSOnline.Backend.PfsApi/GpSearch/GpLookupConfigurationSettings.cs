using System;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.PfsApi.GpSearch
{
    public class GpLookupConfigurationSettings
    {
        public int GpLookupApiResultsLimit { get; }
        public Uri NhsSearchBaseUrl { get; }
        public string GpLookupApiKey { get; }
        public string PostcodeLookupSearchRadiusKm { get; }
        
        public GpLookupConfigurationSettings(int resultsLimit, Uri nhsSearchBaseUrl, string gpLookupApiKey, string postcodeLookupSearchRadiusKm)
        {
            GpLookupApiResultsLimit = resultsLimit;
            NhsSearchBaseUrl = nhsSearchBaseUrl;
            GpLookupApiKey = gpLookupApiKey;
            PostcodeLookupSearchRadiusKm = postcodeLookupSearchRadiusKm;
        }

        public void Validate() 
        {
            if (GpLookupApiKey == null)
            {
                throw new ConfigurationNotFoundException(nameof(GpLookupApiKey));
            }

            if (PostcodeLookupSearchRadiusKm != null) 
            {
                throw new ConfigurationNotFoundException(nameof(PostcodeLookupSearchRadiusKm));
            }

            if (GpLookupApiResultsLimit <= 0) 
            {
                throw new ConfigurationNotFoundException(nameof(GpLookupApiKey));
            }
        }
    }
}