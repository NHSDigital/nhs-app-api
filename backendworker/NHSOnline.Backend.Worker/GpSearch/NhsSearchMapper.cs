using NHSOnline.Backend.Worker.Areas.GpSearch.Models;
using NHSOnline.Backend.Worker.GpSearch.Models;

namespace NHSOnline.Backend.Worker.GpSearch
{
    public class NhsSearchMapper: INhsSearchMapper
    {
        public GpSearchResponse Map(NhsOrganisationSearchResponse nhsSearchResponse)
        {
            return new GpSearchResponse
            {
                Organisations = nhsSearchResponse.Organisations,
                OrganisationQueryCount = nhsSearchResponse.OrganisationCount,
            };
        }
    }
}