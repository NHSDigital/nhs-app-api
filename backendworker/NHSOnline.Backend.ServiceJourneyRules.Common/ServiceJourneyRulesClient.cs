using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ServiceJourneyRules.Common.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.Support.ResponseParsers;

namespace NHSOnline.Backend.ServiceJourneyRules.Common
{
    public class ServiceJourneyRulesClient : IServiceJourneyRulesClient
    {
        private readonly ILogger<ServiceJourneyRulesClient> _logger;
        private readonly ServiceJourneyRulesHttpClient _serviceJourneyRulesHttpClient;
        private readonly IJsonResponseParser _responseParser;

        private const string ServiceJourneyRulesPath = "api/servicejourneyrules";

        public ServiceJourneyRulesClient(
            ILogger<ServiceJourneyRulesClient> logger,
            ServiceJourneyRulesHttpClient service,
            IJsonResponseParser responseParser)
        {
            _logger = logger;
            _serviceJourneyRulesHttpClient = service;
            _responseParser = responseParser;
        }

        public async Task<ServiceJourneyRulesApiObjectResponse<ServiceJourneyRulesResponse>> GetServiceJourneyRules(string odsCode)
        {
            return await Get<ServiceJourneyRulesResponse>(odsCode);
        }

        private async Task<ServiceJourneyRulesApiObjectResponse<TResponse>> Get<TResponse>(string odsCode)
        {

            var path = string.IsNullOrWhiteSpace(odsCode)
                ? $"{ServiceJourneyRulesPath}/no-ods"
                : $"{ServiceJourneyRulesPath}?odsCode={odsCode}";

            using (var request = new HttpRequestMessage(HttpMethod.Get, path))
            {
                return await SendRequestAndParseResponse<TResponse>(request);
            }
        }

        private async Task<ServiceJourneyRulesApiObjectResponse<TResponse>> SendRequestAndParseResponse<TResponse>(
            HttpRequestMessage request)
        {
            var responseMessage = await _serviceJourneyRulesHttpClient.Client.SendAsync(request);
            var response = new ServiceJourneyRulesApiObjectResponse<TResponse>(responseMessage.StatusCode);
            return await response.Parse(responseMessage, _responseParser, _logger);
        }
    }
}