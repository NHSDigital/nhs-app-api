using NHSOnline.Backend.PfsApi.GpSearch.Models;
using NHSOnline.Backend.PfsApi.GpSearch.Models.Pharmacy;

namespace NHSOnline.Backend.PfsApi.GpSearch
{
    public interface INhsSearchResultChecker
    {
        GpSearchResult Check(
            GpLookupClient.NhsSearchApiObjectResponse<NhsOrganisationSearchResponse> nhsSearchResponse, string postcode);
        
        PharmacySearchResponse CheckPharmacies(
            GpLookupClient.NhsSearchApiObjectResponse<NhsOrganisationSearchResponse> pharmacySearchResponse, string postcode);
            
    }
}