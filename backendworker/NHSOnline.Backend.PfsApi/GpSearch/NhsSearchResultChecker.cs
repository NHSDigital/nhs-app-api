using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.GpSearch.Models;

namespace NHSOnline.Backend.PfsApi.GpSearch
{   
    public class NhsSearchResultChecker : INhsSearchResultChecker
    {
        private readonly ILogger<NhsSearchResultChecker> _logger;
        
        public NhsSearchResultChecker(ILogger<NhsSearchResultChecker> logger)
        {
            _logger = logger;
        }

        public GpSearchResult Check(GpLookupClient.NhsSearchApiObjectResponse<NhsOrganisationSearchResponse> nhsSearchResponse, string postcode)
        {
            if (!nhsSearchResponse.HasSuccessResponse)
            {
                _logger.LogError(
                    $"Unsuccessful request searching for Gp Practice by Latitude and Longitude Failed for postcode {postcode}," +
                    $" Status code: {(int) nhsSearchResponse.StatusCode}");
                return new GpSearchResult.InternalServerError();
            }

            if (nhsSearchResponse.Body == null)
            {
                _logger.LogError(
                    $"Search for Nhs GP Practice by Latitude and Longitude Failed for postcode {postcode}," +
                    $" no response body found");
                return new GpSearchResult.InternalServerError();
            }

            var searchResponse = new GpSearchResponse
            {
                Organisations = nhsSearchResponse.Body.Organisations,
                OrganisationQueryCount = nhsSearchResponse.Body.OrganisationCount,
            };
                
            _logger.LogInformation($"{searchResponse.OrganisationQueryCount} results return for search: {postcode}");
            return new GpSearchResult.Success(searchResponse);
        }
    }
}
