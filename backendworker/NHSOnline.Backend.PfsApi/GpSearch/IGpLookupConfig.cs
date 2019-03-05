using System;

namespace NHSOnline.Backend.PfsApi.GpSearch
{
    public interface IGpLookupConfig
    {         
        Uri NhsSearchBaseUrl { get; }
        int GpLookupApiResultsLimit { get; }
        string GpLookupApiKey { get; }
        string PostcodeLookupSearchRadiusKm { get; }
    }
}