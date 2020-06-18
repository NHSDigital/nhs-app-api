using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.Extensions.Logging;
using GeoCoordinatePortable;
using NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy;
using NHSOnline.Backend.PfsApi.GpSearch.Models;
using NHSOnline.Backend.PfsApi.GpSearch.Models.Pharmacy;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using System.Collections.Generic;
using NHSOnline.Backend.NominatedPharmacy.Models;

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

        public async Task<PharmacySearchResult> SearchOnlineOnlyPharmacies()
        {
            _logger.LogEnter();

            var query = CreateOnlinePharmacyOnlySearchQuery(null);
            try
            {
                var searchResults = await _gpLookupClient.OrganisationSearch(query);

                var pharmacySearchResponse = _nhsSearchResultChecker.CheckPharmacies(searchResults);

                if (pharmacySearchResponse.StatusCode == HttpStatusCode.OK)
                {
                    var pharmaciesInRandomOrder = PickContactableOnlinePharmacies(pharmacySearchResponse.Pharmacies)
                        .InRandomOrder()
                        .Take(_gpLookupConfig.OnlinePharmacyRandomisedSearchResultLimit);

                    try
                    {
                        var pharmacies = _pharmacyDetailsToPharmacyDetailsResponseMapper.Map(pharmaciesInRandomOrder);

                        return new PharmacySearchResult.Success(pharmacies);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"Error during mapping list of {nameof(Organisation)} to list of {nameof(PharmacyDetailsResponse)}");
                        return new PharmacySearchResult.InternalServerError();
                    }
                }

                return new PharmacySearchResult.InternalServerError();

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error processing online pharmacy search");
                return new PharmacySearchResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<PharmacySearchResult> SearchOnlineOnlyPharmacies(string searchTerm)
        {
            _logger.LogEnter();

            searchTerm = OrganisationSearchUtility.SanitizeOnlinePharmacyNameSearch(searchTerm);

            var isValid = new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(searchTerm, nameof(searchTerm))
                .IsValid();

            if (!isValid)
            {
                return new PharmacySearchResult.BadRequest();
            }

            searchTerm = OrganisationSearchUtility.PrepareSearch(searchTerm);

            var query = CreateOnlinePharmacyOnlySearchQuery(searchTerm);
            try
            {
                var searchResults = await _gpLookupClient.OrganisationSearch(query);

                var pharmacySearchResponse = _nhsSearchResultChecker.CheckPharmacies(searchResults);

                if (pharmacySearchResponse.StatusCode == HttpStatusCode.OK)
                {
                    var contactablePharmacies = PickContactableOnlinePharmacies(pharmacySearchResponse.Pharmacies).ToList();

                    var nonContactablePharmaciesCount = pharmacySearchResponse.Pharmacies.Count - contactablePharmacies.Count;

                    try
                    {
                        var pharmacies = _pharmacyDetailsToPharmacyDetailsResponseMapper.Map(contactablePharmacies);

                        return new PharmacySearchResult.Success(
                            pharmacies,
                            pharmacySearchResponse.PharmacyCount - nonContactablePharmaciesCount);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"Error during mapping list of {nameof(Organisation)} to list of {nameof(PharmacyDetailsResponse)}");
                        return new PharmacySearchResult.InternalServerError();
                    }
                }

                return new PharmacySearchResult.InternalServerError();

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error processing online pharmacy search");
                return new PharmacySearchResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<PharmacySearchResult> Search(string searchTerm)
        {
            try
            {
                _logger.LogEnter();

                var sanitizedSearchTerm = SanitizeAndValidateSearchTerm(searchTerm);
                if (sanitizedSearchTerm.Failed(out var sanitizedSearchTermFailure))
                {
                    return sanitizedSearchTermFailure;
                }

                var parsedPostcode = ParsePostcodeData(sanitizedSearchTerm);
                if (parsedPostcode.Failed(out var parsedPostcodeFailure))
                {
                    return parsedPostcodeFailure;
                }

                var postcodeData = await LookupPostcodeCoordinates(sanitizedSearchTerm, parsedPostcode);
                if (postcodeData.Failed(out var postcodeDataFailure))
                {
                    return postcodeDataFailure;
                }

                var pharmacies = await FindPharmaciesByPostcode(sanitizedSearchTerm, parsedPostcode, postcodeData);
                if (pharmacies.Failed(out var pharmaciesFailure))
                {
                    return pharmaciesFailure;
                }

                return new PharmacySearchResult.Success(pharmacies.Result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing Postcode search input: {searchTerm}");
                return new PharmacySearchResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private ProcessResult<string, PharmacySearchResult> SanitizeAndValidateSearchTerm(string searchTerm)
        {
            searchTerm = OrganisationSearchUtility.SanitizeSearch(searchTerm);

            var isValid = new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(searchTerm, nameof(searchTerm))
                .IsValid();

            if (!isValid)
            {
                return new PharmacySearchResult.BadRequest();
            }

            return searchTerm;
        }

        private ProcessResult<ParsedPostcode, PharmacySearchResult> ParsePostcodeData(string searchTerm)
        {
            var postcodeDetail = _postcodeParser.ParseSearchTermForPostcodeMatch(searchTerm);

            if (!postcodeDetail.IsPostcode)
            {
                _logger.LogInformation($"Didn't recognise as valid postcode: {searchTerm}");
                return new PharmacySearchResult.InvalidPostcode();
            }

            return postcodeDetail;
        }

        private async Task<ProcessResult<PostcodeData, PharmacySearchResult>> LookupPostcodeCoordinates(
            string searchTerm,
            ParsedPostcode parsedPostcode)
        {
            try
            {
                var search = OrganisationSearchUtility.PrepareSearch(parsedPostcode.Postcode, true);
                var postcodeSearchData =
                    OrganisationSearchUtility.CreatePostcodeSearchQuery(search,
                        string.IsNullOrEmpty(parsedPostcode.Inward));

                var postcodeSearchResult = await _gpLookupClient.PostcodeSearch(postcodeSearchData);

                if (!postcodeSearchResult.HasSuccessResponse)
                {
                    _logger.LogError($"Unsuccessful request searching for Postcode Latitude and Longitude " +
                                     $"on Nhs Search Service, Status code: " +
                                     $"{(int) postcodeSearchResult.StatusCode}");

                    return new PharmacySearchResult.PostcodeResultFailure(postcodeSearchResult.StatusCode);
                }

                var postcodeCoordinates = postcodeSearchResult.Body?.PostcodeData?.FirstOrDefault();

                if (postcodeCoordinates == null)
                {
                    _logger.LogInformation($"NHS Search service returned no postcode data for: {searchTerm}");
                    return new PharmacySearchResult.PostcodeResultFailure(postcodeSearchResult.StatusCode);
                }

                return postcodeCoordinates;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Search for Nhs pharmacies Failed for: {searchTerm} ");
                return new PharmacySearchResult.InternalServerError();
            }
        }

        private async Task<ProcessResult<IEnumerable<PharmacyDetails>, PharmacySearchResult>> FindPharmaciesByPostcode(
            string searchTerm,
            ParsedPostcode postcodeDetail,
            PostcodeData postcodeCoordinates)
        {
            try
            {
                var organisationSearchQuery = CreateOrganisationPostcodeSearchQuery(postcodeCoordinates);

                var pharmacySearchResponse =
                    await ExecuteOrganisationPostcodeSearch(organisationSearchQuery, postcodeDetail.Postcode);

                pharmacySearchResponse.PostcodeCoordinate = new GeoCoordinate(
                    double.Parse(postcodeCoordinates.Latitude, CultureInfo.InvariantCulture),
                    double.Parse(postcodeCoordinates.Longitude, CultureInfo.InvariantCulture));

                try
                {
                    var pharmacies = _pharmacyDetailsToPharmacyDetailsResponseMapper.Map(
                        pharmacySearchResponse.Pharmacies,
                        pharmacySearchResponse.PostcodeCoordinate);

                    return ProcessResult<IEnumerable<PharmacyDetails>, PharmacySearchResult>.FromTResult(pharmacies);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error during mapping list of {nameof(Organisation)} to list of {nameof(PharmacyDetailsResponse)}");
                    var finalResult = new PharmacySearchResult.InternalServerError();
                    return finalResult;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Search for Nhs pharmacies Failed for: {searchTerm} ");
                var finalResult = new PharmacySearchResult.InternalServerError();
                return finalResult;
            }
        }

        private static IEnumerable<Organisation> PickContactableOnlinePharmacies(IEnumerable<Organisation> pharmacies)
        {
            return pharmacies.Where(x => !string.IsNullOrEmpty(x.URL)
                                         || x.GetContactsArray().Any(c =>
                                             c.OrganisationContactMethodType ==
                                             ResponseEnums.OrganisationContactMethodType.Telephone &&
                                             !string.IsNullOrEmpty(c.OrganisationContactValue))).ToList();
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

        private OrganisationSearchData CreateOnlinePharmacyOnlySearchQuery(string searchTerm)
        {
            var searchData = new OrganisationSearchData
            {
                Select = "OrganisationName,URL,Contacts,NACSCode",
                Filter = $"(OrganisationTypeID eq '{Constants.OrganisationTypePharmacy}') and (OrganisationSubType eq '{Constants.OrganisationSubTypeForInternetPharmacy}')",
                Count = true,
            };

            if (string.IsNullOrEmpty(searchTerm))
            {
                // a big number to make sure all online pharmacies are
                // included, otherwise NHS search defaults to 50
                searchData.Top = 1000;
            }
            else
            {
                searchData.Search = searchTerm;
                searchData.SearchFields = "OrganisationName";
                searchData.Top = _gpLookupConfig.OnlinePharmacySearchResultLimit;
            }

            return searchData;
        }
    }
}

