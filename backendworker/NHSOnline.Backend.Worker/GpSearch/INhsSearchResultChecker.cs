using NHSOnline.Backend.Worker.GpSearch.Models;

namespace NHSOnline.Backend.Worker.GpSearch
{
    public interface INhsSearchResultChecker
    {
        GpSearchResult Check(
            GpLookupClient.NhsSearchApiObjectResponse<NhsOrganisationSearchResponse> nhsSearchResponse, string postcode);
    }
}