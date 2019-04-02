using NHSOnline.Backend.PfsApi.GpSearch.Models;

namespace NHSOnline.Backend.PfsApi.GpSearch
{
    public interface INhsSearchMapper
    {
        GpSearchResponse Map(NhsOrganisationSearchResponse nhsSearchResponse);          
    }
}