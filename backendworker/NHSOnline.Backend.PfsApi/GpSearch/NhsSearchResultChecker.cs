using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.GpSearch.Models;
using NHSOnline.Backend.PfsApi.GpSearch.Models.Pharmacy;
using NHSOnline.Backend.Support.Logging;

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

        public GpSearchResult CheckOdsCodeSearchResult(
            GpLookupClient.NhsSearchApiObjectResponse<NhsOrganisationSearchResponse> nhsSearchResponse,
            string odsCode)
        {
            if (!nhsSearchResponse.HasSuccessResponse)
            {
                _logger.LogError(
                    $"Unsuccessful request searching for Gp Practice by ods code {odsCode}," +
                    $" Status code: {(int) nhsSearchResponse.StatusCode}");
                return new GpSearchResult.Unsuccessful();
            }

            if (nhsSearchResponse.Body == null)
            {
                _logger.LogError(
                    $"Search for Nhs GP Practice by ods code {odsCode}, no response body found");
                return new GpSearchResult.Unsuccessful();
            }

            var searchResponse = new GpSearchResponse
            {
                Organisations = nhsSearchResponse.Body.Organisations,
                OrganisationQueryCount = nhsSearchResponse.Body.OrganisationCount,
            };

            _logger.LogInformation($"{searchResponse.OrganisationQueryCount} results return for search: {odsCode}");
            return new GpSearchResult.SuccessfullyRetrieved(searchResponse);
        }

        public PharmacySearchResponse CheckPharmacies(GpLookupClient.NhsSearchApiObjectResponse<NhsOrganisationSearchResponse> pharmacySearchResponse, string postcode)
        {
            if (!pharmacySearchResponse.HasSuccessResponse)
            {
                _logger.LogError(
                    $"Unsuccessful request searching for Pharmacies by Latitude and Longitude Failed for postcode {postcode}," +
                    $" Status code: {(int) pharmacySearchResponse.StatusCode}");
                return new PharmacySearchResponse(pharmacySearchResponse.StatusCode);
            }

            if (pharmacySearchResponse.Body == null)
            {
                _logger.LogError(
                    $"Search for Nhs Pharmacy by Latitude and Longitude Failed for postcode {postcode}," +
                    $" no response body found");
                return new PharmacySearchResponse(pharmacySearchResponse.StatusCode);
            }

            var logDetail = new Dictionary<string, string>
            {
                { "total count available", pharmacySearchResponse.Body.OrganisationCount.ToString(CultureInfo.CurrentCulture) },
                { "number of items returned", pharmacySearchResponse.Body.Organisations.Count.ToString(CultureInfo.CurrentCulture) },
            };

            _logger.LogInformationKeyValuePairs("Pharmacy postcode search summary", logDetail);
            
            var searchResponse = new PharmacySearchResponse(
                pharmacySearchResponse.StatusCode,
                pharmacySearchResponse.Body.Organisations);

            return searchResponse;
        }
    }
}
