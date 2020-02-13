using System;

namespace NHSOnline.Backend.PfsApi.GpSearch
{
    public interface IGpLookupConfig
    {
        Uri NhsSearchBaseUrl { get; }

        int PharmacySearchApiLimit { get; }

        int OnlinePharmacyRandomisedSearchResultLimit { get; }
        
        int OnlinePharmacySearchResultLimit { get; }

        string GpLookupApiKey { get; }

        /// <summary>
        /// Required for testing as our test ODS codes do not exist in NHS search results.
        /// </summary>
        string GpPracticeOdsCodeForEpsEnabledCheckOverride { get; }
    }
}
