using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.GpSearch.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.GpSearch
{
    public class GpSearchService: IGpSearchService
    {
        private readonly ILogger<GpSearchService> _logger;
        private readonly IGpLookupClient _gpLookupClient;
        private readonly IGpLookupConfig _gpLookupConfig;
        private readonly INhsSearchResultChecker _nhsSearchResultChecker;
        private readonly IPostcodeParser _postcodeParser;
        
        public GpSearchService(
            ILogger<GpSearchService> logger,
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

        public async Task<GpSearchResult> Search(string searchTerm)
        {
            _logger.LogEnter();
            
            searchTerm = SanitizeSearch(searchTerm);
           
            var isValid = new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(searchTerm, nameof(searchTerm))
                .IsValid();

            if (!isValid)
            {
                return new GpSearchResult.BadRequest();
            }

            var postcodeMatch = _postcodeParser.ParseSearchTermForPostcodeMatch(searchTerm);

            if (!postcodeMatch.IsPostcode)
            {
                searchTerm = PrepareSearch(searchTerm);
                var data = GetOrganisationSearchData(searchTerm);
                return await ExecuteOrganisationSearch(data);
            }

            try
            {
                var search = PrepareSearch(postcodeMatch.Postcode, true);

                var postcodeSearchData = GetPostcodeSearchData(search, string.IsNullOrEmpty(postcodeMatch.Inward));

                try
                {
                    var postcodeSearchResult = await _gpLookupClient.PostcodeSearch(postcodeSearchData);

                    _logger.LogEnter();

                    if (!postcodeSearchResult.HasSuccessResponse)
                    {
                        _logger.LogError($"Unsuccessful request searching for Postcode Latitude and Longitude on Nhs Search Service, Status code: " +
                                         $"{(int)postcodeSearchResult.StatusCode}");
                        return new GpSearchResult.InternalServerError();
                    }

                    var postcodeData = postcodeSearchResult?.Body?.PostcodeDatas?.FirstOrDefault();

                    if (postcodeData == null)
                    {
                        _logger.LogInformation($"NHS Search service returned no postcode data for: {searchTerm} ");
                        return new GpSearchResult.Success(new GpSearchResponse());
                    }

                    var organisationSearchData = GetOrganisationPostcodeSearchData(postcodeData);
                    
                    return await ExecuteOrganisationPostcodeSearch(organisationSearchData, postcodeMatch.Postcode);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, $"Search for Nhs GP Practice Failed for: {searchTerm} ");
                    return new GpSearchResult.BadGateway();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing Postcode search input: {searchTerm} ");
                return new GpSearchResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task<GpSearchResult> ExecuteOrganisationSearch(OrganisationSearchData organisationSearchData)
        {
            try
            {
                _logger.LogEnter();
                
                var searchResults = await _gpLookupClient.GpSearch(organisationSearchData);

                return _nhsSearchResultChecker.Check(searchResults, organisationSearchData.Search);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Search for Nhs GP Practice Failed: {organisationSearchData.Search} ");
                return new GpSearchResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }
        
        private async Task<GpSearchResult> ExecuteOrganisationPostcodeSearch(OrganisationPostcodeSearchData organisationSearchData, string postcode)
        {
            try
            {
                _logger.LogEnter();

                var searchResults = await _gpLookupClient.GpPostcodeSearch(organisationSearchData);

                return _nhsSearchResultChecker.Check(searchResults, postcode);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex,
                    $"Search for Nhs GP Practice by Latitude and Longitude Failed for postcode: {postcode}");
                return new GpSearchResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }
        
        private OrganisationPostcodeSearchData GetOrganisationPostcodeSearchData(PostcodeData postcodeData)
        {
            return new OrganisationPostcodeSearchData
            {
                Top = _gpLookupConfig.GpLookupApiResultsLimit,
                Select = "OrganisationID,OrganisationName,Address1,Address2,Address3,City,County,Postcode,NACSCode,Geocode",
                Filter = "OrganisationTypeID eq 'GPB' " +
                         $"and geo.distance(Geocode, geography'POINT({postcodeData.Longitude} {postcodeData.Latitude})') le {_gpLookupConfig.PostcodeLookupSearchRadiusKm}",
                OrderBy = $"geo.distance(Geocode, geography'POINT({postcodeData.Longitude} {postcodeData.Latitude})')",
                Count = true,
            };
        }
        
        private OrganisationSearchData GetOrganisationSearchData(string searchTerm)
        {
            return new OrganisationSearchData
            {
                Top = _gpLookupConfig.GpLookupApiResultsLimit,
                Search = searchTerm,
                SearchFields = "OrganisationName,Address2,Address3,City",
                Select = "OrganisationID,OrganisationName,Address1,Address2,Address3,City,County,Postcode,NACSCode",
                Filter = "OrganisationTypeID eq 'GPB'",
                QueryType = "simple",
                Count = true,
            };
        }

        private static PostcodeSearchData GetPostcodeSearchData(string searchTerm, bool outcodeOnly)
        {
            return new PostcodeSearchData
            {
                Top = 1,
                Search = searchTerm,
                Count = true,
                Filter = GetPostcodesAndPlacesFilter(outcodeOnly),
            };
        }

        private static string GetPostcodesAndPlacesFilter(bool outcodeOnly)
        {
            return outcodeOnly ? "Type eq 'PostcodeOutCode'" : "LocalType eq 'Postcode'";
        }
        
        private static string SanitizeSearch(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return searchTerm;
            }
            
            var dashesRemoved = searchTerm.Trim().Replace('-', ' ');
            var multipleSpacesRemoved = Regex.Replace(dashesRemoved, "\\s\\s+/g", " ");
            var sanitisedSearchCriteria = Regex.Replace(multipleSpacesRemoved, "[/\\\\^$*+&?,.()|[\\]{}\"~:!<>£;@%^'`]/g", string.Empty);
            
            return sanitisedSearchCriteria;
        }
        
        private static string PrepareSearch(string searchTerm, bool matchWhole = false)
        {
            if (matchWhole)
            {
                return $"\"{searchTerm}\"";
            }

            var parts = searchTerm.Split(' ').Join("*+");
            return $"{parts}*";
        }
    }
}