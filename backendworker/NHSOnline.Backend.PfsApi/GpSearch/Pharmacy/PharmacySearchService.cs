using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.Extensions.Logging;
using GeoCoordinatePortable;
using NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy;
using NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy.Models;
using NHSOnline.Backend.PfsApi.GpSearch.Models;
using NHSOnline.Backend.PfsApi.GpSearch.Models.Pharmacy;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.GpSearch.Pharmacy
{
    public class PharmacySearchService : IPharmacySearchService
    {
        private readonly ILogger<PharmacySearchService> _logger;
        private readonly IGpLookupClient _gpLookupClient;
        private readonly IGpLookupConfig _gpLookupConfig;
        private readonly INhsSearchResultChecker _nhsSearchResultChecker;
        private readonly IPostcodeParser _postcodeParser;
        private readonly IPharmacyDetailsToPharmacyDetailsResponseMapper _pharmacyDetailsToPharmacyDetailsResponseMapper;


        public PharmacySearchService(
            ILogger<PharmacySearchService> logger,
            IGpLookupClient gpLookupClient,
            IGpLookupConfig gpLookupConfig,
            INhsSearchResultChecker nhsSearchResultChecker,
            IPostcodeParser postcodeParser,
            IPharmacyDetailsToPharmacyDetailsResponseMapper pharmacyDetailsToPharmacyDetailsResponseMapper
            )
        {
            _logger = logger;
            _gpLookupClient = gpLookupClient;
            _gpLookupConfig = gpLookupConfig;
            _nhsSearchResultChecker = nhsSearchResultChecker;
            _postcodeParser = postcodeParser;
            _pharmacyDetailsToPharmacyDetailsResponseMapper = pharmacyDetailsToPharmacyDetailsResponseMapper;
        }

        public async Task<PharmacySearchResult> Search(string searchTerm)
        {
            _logger.LogEnter();

            searchTerm = OrganisationSearchUtility.SanitizeSearch(searchTerm);

            var isValid = new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(searchTerm, nameof(searchTerm))
                .IsValid();

            if (!isValid)
            {
                return new PharmacySearchResult.BadRequest();
            }

            var postcodeDetail = _postcodeParser.ParseSearchTermForPostcodeMatch(searchTerm);

            if (!postcodeDetail.IsPostcode)
            {
                _logger.LogInformation($"Didn't recognise as valid postcode: {searchTerm}");
                return new PharmacySearchResult.InvalidPostcode();
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
                                         $"{(int) postcodeSearchResult.StatusCode}");
                        
                        return new PharmacySearchResult.PostcodeResultFailure(postcodeSearchResult.StatusCode);
                    }

                    var postcodeCoordinates = postcodeSearchResult?.Body?.PostcodeData?.FirstOrDefault();

                    if (postcodeCoordinates == null)
                    {
                        _logger.LogInformation($"NHS Search service returned no postcode data for: {searchTerm}");
                        return new PharmacySearchResult.PostcodeResultFailure(postcodeSearchResult.StatusCode);
                    }

                    var organisationSearchQuery = CreateOrganisationPostcodeSearchQuery(postcodeCoordinates);
                    
                    var pharmacySearchResponse = 
                        await ExecuteOrganisationPostcodeSearch(organisationSearchQuery, postcodeDetail.Postcode);


                    pharmacySearchResponse.PostcodeCoordinate = new GeoCoordinate(
                        double.Parse(postcodeCoordinates.Latitude, CultureInfo.InvariantCulture),
                        double.Parse(postcodeCoordinates.Longitude, CultureInfo.InvariantCulture));

                    var pharmacies = Enumerable.Empty<PharmacyDetails>();
                    try
                    {
                         pharmacies = _pharmacyDetailsToPharmacyDetailsResponseMapper.Map(
                            pharmacySearchResponse.Pharmacies,
                            pharmacySearchResponse.PostcodeCoordinate);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"Error during mapping list of {nameof(Organisation)} to list of {nameof(PharmacyDetailsResponse)}");
                        return new PharmacySearchResult.InternalServerError();
                    } 

                    return new PharmacySearchResult.Success(pharmacies);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, $"Search for Nhs pharmacies Failed for: {searchTerm} ");
                    return new PharmacySearchResult.InternalServerError();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing Postcode search input: {searchTerm} ");
                return new PharmacySearchResult.InternalServerError();
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
                    $"Search for Nhs Pharmacies by Latitude and Longitude Failed for search term: {postcode}");
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
                Filter = $"OrganisationSubType eq '{ Constants.OrganisationSubTypeForCommunityPharmacy }' ",
                OrderBy = $"geo.distance(Geocode, geography'POINT({postcodeData.Longitude} {postcodeData.Latitude})')",
                Search = $"Metrics:({ Constants.MetricIdForEPSEnabled })",
                Count = true,
                QueryType = "full",
                SearchMode = "all"
            };
        }
    }
}

