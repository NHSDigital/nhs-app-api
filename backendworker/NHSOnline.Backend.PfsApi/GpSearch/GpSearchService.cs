using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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

        public GpSearchService(
            ILogger<GpSearchService> logger,
            IGpLookupClient gpLookupClient,
            IGpLookupConfig gpLookupConfig,
            INhsSearchResultChecker nhsSearchResultChecker)
        {
            _logger = logger;
            _gpLookupClient = gpLookupClient;
            _gpLookupConfig = gpLookupConfig;
            _nhsSearchResultChecker = nhsSearchResultChecker;
        }

        public async Task<IsGpPracticeEpsEnabledResponse> IsGpPracticeEPSEnabled(string odsCode)
        {
            _logger.LogEnter();

            if (!string.IsNullOrEmpty(_gpLookupConfig.GpPracticeOdsCodeForEpsEnabledCheckOverride))
            {
                _logger.LogInformation($"Overriding ods code from { odsCode } to { _gpLookupConfig.GpPracticeOdsCodeForEpsEnabledCheckOverride } for { nameof(IsGpPracticeEPSEnabled)} check");
                odsCode = _gpLookupConfig.GpPracticeOdsCodeForEpsEnabledCheckOverride;
            }

            var isValid = new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(odsCode, nameof(odsCode))
                .IsValid();

            if (!isValid)
            {
                return new IsGpPracticeEpsEnabledResponse(HttpStatusCode.BadRequest);
            }

            var data = GetSearchByOdsCodeOrganisationSearchData(odsCode);
            var searchResponse = await _gpLookupClient.OrganisationSearch(data);

            if (!searchResponse.HasSuccessResponse)
            {
                _logger.LogError(
                    $"Unsuccessful request searching for Gp Practice by ods code {odsCode}," +
                    $" Status code: {(int) searchResponse.StatusCode}");
                return new IsGpPracticeEpsEnabledResponse(HttpStatusCode.BadGateway);
            }

            if (searchResponse.Body == null)
            {
                _logger.LogError(
                    $"Search for Nhs GP Practice by ods code {odsCode}, no response body found");
                return new IsGpPracticeEpsEnabledResponse(HttpStatusCode.NotFound);
            }

            if (searchResponse.Body.Organisations == null || !searchResponse.Body.Organisations.Any())
            {
                _logger.LogError(
                    $"Search for Nhs GP Practice by ods code {odsCode}, no organisations found for search");
                return new IsGpPracticeEpsEnabledResponse(HttpStatusCode.NotFound);
            }

            var metrics = searchResponse.Body.Organisations.First().GetMetricsArray();

            if (!metrics.Any(x => x.MetricID == Constants.MetricIdForEPSEnabled))
            {
                _logger.LogInformation(
                    $"Search for Nhs GP Practice by ods code {odsCode}, not enabled for eps ");
                return new IsGpPracticeEpsEnabledResponse(HttpStatusCode.OK, false);
            }

            return new IsGpPracticeEpsEnabledResponse(HttpStatusCode.OK, true);
        }

        public async Task<GpSearchResult> GetGpPracticeByOdsCode(string odsCode)
        {
            _logger.LogEnter();

            var isValid = new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(odsCode, nameof(odsCode))
                .IsValid();

            if (!isValid)
            {
                return new GpSearchResult.BadRequest();
            }

            var data = GetSearchByOdsCodeOrganisationSearchData(odsCode);
            return await ExecuteOrganisationSearch(data);
        }

        private async Task<GpSearchResult> ExecuteOrganisationSearch(OrganisationSearchData organisationSearchData)
        {
            try
            {
                _logger.LogEnter();

                var searchResults = await _gpLookupClient.OrganisationSearch(organisationSearchData);

                return _nhsSearchResultChecker.CheckOdsCodeSearchResult(searchResults, organisationSearchData.Search);
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

        private OrganisationSearchData GetSearchByOdsCodeOrganisationSearchData(string odsCode)
        {
            return new OrganisationSearchData
            {
                Top = 1,
                Filter = $"OrganisationTypeID eq '{Constants.OrganisationTypeGpPractice}' and NACSCode eq '{odsCode}'",
                Select = "OrganisationID,OrganisationName,NACSCode,Metrics",
            };
        }
    }
}