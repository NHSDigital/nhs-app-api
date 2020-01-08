using System;

namespace NHSOnline.Backend.PfsApi.GpSearch
{
    public interface IGpLookupConfig
    {
        Uri NhsSearchBaseUrl { get; }

        int GpLookupApiResultsLimit { get; }

        int PharmacySearchApiLimit { get; }

        int OnlinePharmacyRandomisedSearchResultLimit { get; }
        
        int OnlinePharmacySearchResultLimit { get; }

        string GpLookupApiKey { get; }

        string PostcodeLookupSearchRadiusKm { get; }

        /// <summary>
        /// Required for testing as our test ODS codes do not exist in NHS search results.
        /// </summary>
        string GpPraticeOdsCodeForEpsEnabledCheckOverride { get; }
    }
}
