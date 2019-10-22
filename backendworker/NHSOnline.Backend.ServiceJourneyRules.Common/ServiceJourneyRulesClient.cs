using System.Net.Http;
using System.Threading.Tasks;
using CorrelationId;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ServiceJourneyRules.Common.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.ResponseParsers;

namespace NHSOnline.Backend.ServiceJourneyRules.Common
{
    public class ServiceJourneyRulesClient: IServiceJourneyRulesClient
    {
        private readonly ILogger<ServiceJourneyRulesClient> _logger;
        private readonly ServiceJourneyRulesHttpClient _serviceJourneyRulesHttpClient;
        private readonly IJsonResponseParser _responseParser;
        private readonly ICorrelationContextAccessor _correlationContext;

        private const string ServiceJourneyRulesPath = "api/servicejourneyrules";
        
        public ServiceJourneyRulesClient(
            ILogger<ServiceJourneyRulesClient> logger, 
            ServiceJourneyRulesHttpClient service, 
            IJsonResponseParser responseParser, 
            ICorrelationContextAccessor correlationContext)
        {
            _logger = logger;
            _serviceJourneyRulesHttpClient = service;
            _responseParser = responseParser;
            _correlationContext = correlationContext;
        }

        public async Task<ServiceJourneyRulesApiObjectResponse<ServiceJourneyRulesResponse>> GetServiceJourneyRules(string odsCode)
        {
            return await Get<ServiceJourneyRulesResponse>(odsCode);            
        }
        
        private async Task<ServiceJourneyRulesApiObjectResponse<TResponse>> Get<TResponse>(string odsCode)
        {
            var path = $"{ServiceJourneyRulesPath}?odsCode={odsCode}";
            var request = BuildRequest(HttpMethod.Get, path);            
            return await SendRequestAndParseResponse<TResponse>(request);
        }

        private HttpRequestMessage BuildRequest(HttpMethod method, string path)
        {
            var request = new HttpRequestMessage(method, path);
            
            request.Headers.Add(Constants.HttpHeaders.CorrelationIdentifier,
                _correlationContext.CorrelationContext?.CorrelationId ?? string.Empty);

            return request;
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