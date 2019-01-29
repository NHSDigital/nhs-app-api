using NHSOnline.Backend.Worker.Areas.GpSearch.Models;
using NHSOnline.Backend.Worker.GpSearch.Models;

namespace NHSOnline.Backend.Worker.GpSearch
{
    public interface INhsSearchMapper
    {
        GpSearchResponse Map(NhsOrganisationSearchResponse nhsSearchResponse);          
    }
}