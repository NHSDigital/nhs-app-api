using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GeoCoordinatePortable;
using NHSOnline.Backend.PfsApi.GpSearch.Models;
using NHSOnline.Backend.PfsApi.GpSearch.Models.Pharmacy;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.GpSearch.Pharmacy
{
    public class PharmacySearchService : IPharmacySearchService
    {
        const string OrganisationSubTypeForCommunityPharmacy = "Community Pharmacy";

        private readonly ILogger<PharmacySearchService> _logger;
        private readonly IGpLookupClient _gpLookupClient;
        private readonly IGpLookupConfig _gpLookupConfig;
        private readonly INhsSearchResultChecker _nhsSearchResultChecker;
        private readonly IPostcodeParser _postcodeParser;

        public PharmacySearchService(
            ILogger<PharmacySearchService> logger,
            IGpLookupClient gpLookupClient,
            IGpLookupConfig gpLookupConfig,
            INhsSearchResultChecker nhsSearchResultChecker,
            IPostcodeParser postcodeParser)
        {
            _logger = logger;
            _gpLookupClient = gpLookupClient;
            _gpLookupConfig = gpLookupConfig;
            _nhsSearchResultChecker = nhsSearchResultChecker;
            _postcodeParser = postcodeParser;
        }

        public async Task<PharmacySearchResponse> Search(string postcode)
        {
            _logger.LogEnter();

            postcode = OrganisationSearchUtility.SanitizeSearch(postcode);

            var isValid = new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(postcode, nameof(postcode))
                .IsValid();

            if (!isValid)
            {
                return new PharmacySearchResponse(HttpStatusCode.BadRequest);
            }

            var postcodeDetail = _postcodeParser.ParseSearchTermForPostcodeMatch(postcode);

            if (!postcodeDetail.IsPostcode)
            {
                _logger.LogInformation($"Didn't recognise as valid postcode: {postcode}");
                return new PharmacySearchResponse(HttpStatusCode.BadRequest);
            }

            try
            {
                var search = OrganisationSearchUtility.PrepareSearch(postcodeDetail.Postcode, true);
                var postcodeSearchData = OrganisationSearchUtility.CreatePostcodeSearchQuery(search, string.IsNullOrEmpty(postcodeDetail.Inward));

                try
                {
                    var postcodeSearchResult = await _gpLookupClient.PostcodeSearch(postcodeSearchData);

                    if (!postcodeSearchResult.HasSuccessResponse)
                    {
                        _logger.LogError($"Unsuccessful request searching for Postcode Latitude and Longitude " +
                                         $"on Nhs Search Service, Status code: " +
                                         $"{(int)postcodeSearchResult.StatusCode}");
                        return new PharmacySearchResponse(postcodeSearchResult.StatusCode);
                    }

                    var postcodeCoordinates = postcodeSearchResult?.Body?.PostcodeData?.FirstOrDefault();

                    if (postcodeCoordinates == null)
                    {
                        _logger.LogInformation($"NHS Search service returned no postcode data for: {postcode} ");
                        return new PharmacySearchResponse(postcodeSearchResult.StatusCode);
                    }

                    var organisationSearchQuery = CreateOrganisationPostcodeSearchQuery(postcodeCoordinates);

                    var pharmacySearchResponse = await ExecuteOrganisationPostcodeSearch(
                        organisationSearchQuery,
                        postcodeDetail.Postcode);

                    pharmacySearchResponse.PostcodeCoordinate = new GeoCoordinate(
                        double.Parse(postcodeCoordinates.Latitude, CultureInfo.InvariantCulture),
                        double.Parse(postcodeCoordinates.Longitude, CultureInfo.InvariantCulture));

                    return pharmacySearchResponse;
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, $"Search for Nhs pharmacies Failed for: {postcode} ");
                    return new PharmacySearchResponse(HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing Postcode search input: {postcode} ");
                return new PharmacySearchResponse(HttpStatusCode.InternalServerError);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task<PharmacySearchResponse> ExecuteOrganisationPostcodeSearch(OrganisationPostcodeSearchData organisationSearchData, string postcode)
        {
            try
            {
                _logger.LogEnter();

                var searchResults = await _gpLookupClient.GpPostcodeSearch(organisationSearchData);

                return _nhsSearchResultChecker.CheckPharmacies(searchResults, postcode);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex,
                    $"Search for Nhs Pharmacies by Latitude and Longitude Failed for postcode: {postcode}");
                return new PharmacySearchResponse(HttpStatusCode.ServiceUnavailable);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private OrganisationPostcodeSearchData CreateOrganisationPostcodeSearchQuery(PostcodeData postcodeData)
        {
            return new OrganisationPostcodeSearchData
            {
                Top = _gpLookupConfig.PharmacySearchApiLimit,
                Select = "OrganisationID,OrganisationName,Address1,Address2,Address3,City,Postcode,NACSCode,Geocode,Contacts,OpeningTimes",
                Filter = $"OrganisationSubType eq '{ OrganisationSubTypeForCommunityPharmacy }' " +
                         $"and geo.distance(Geocode, geography'POINT({postcodeData.Longitude} {postcodeData.Latitude})') le {_gpLookupConfig.PharmacySearchRadiusKm}",
                OrderBy = $"geo.distance(Geocode, geography'POINT({postcodeData.Longitude} {postcodeData.Latitude})')",
                Count = true,
            };
        }
    }
}
