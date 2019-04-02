using NHSOnline.Backend.PfsApi.GpSearch.Models;

namespace NHSOnline.Backend.PfsApi.GpSearch
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