using System.Threading.Tasks;
using NHSOnline.Backend.Worker.GpSearch.Models;

namespace NHSOnline.Backend.Worker.GpSearch
{
    public interface IGpLookupClient
    {
        Task<GpLookupClient.NhsSearchApiObjectResponse<NhsOrganisationSearchResponse>> 
            GpSearch(OrganisationSearchData searchData);
        
        Task<GpLookupClient.NhsSearchApiObjectResponse<NhsOrganisationSearchResponse>> 
            GpPostcodeSearch(OrganisationPostcodeSearchData searchData);
        
        Task<GpLookupClient.NhsSearchApiObjectResponse<NhsPostcodeSearchResponse>> 
            PostcodeSearch(PostcodeSearchData searchData);
    }
}