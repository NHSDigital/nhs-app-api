using System.Threading.Tasks;
using NHSOnline.Backend.PfsApi.GpSearch.Models;

namespace NHSOnline.Backend.PfsApi.GpSearch
{
    public interface IGpLookupClient
    {
        Task<GpLookupClient.NhsSearchApiObjectResponse<NhsOrganisationSearchResponse>> 
            GpSearch(OrganisationSearchData searchData);
        
        Task<GpLookupClient.NhsSearchApiObjectResponse<NhsOrganisationSearchResponse>> 
            GpPostcodeSearch(OrganisationPostcodeSearchData searchData);
        
        Task<GpLookupClient.NhsSearchApiObjectResponse<NhsPostcodeSearchResponse>> 
            PostcodeSearch(PostcodeSearchData searchData);

        Task<GpLookupClient.NhsSearchApiObjectResponse<NhsOrganisationSearchResponse>>
            PharmacySearch(OrganisationSearchData searchData);
    }
}