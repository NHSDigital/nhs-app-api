using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.SecondaryCare.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.ResponseParsers;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public class SecondaryCareClient : ISecondaryCareClient
    {
        private const string SummaryPath = "summary/$evaluate";

        private readonly SecondaryCareHttpClient _httpClient;
        private readonly IJsonResponseParser _responseParser;
        private readonly ILogger<SecondaryCareClient> _logger;

        public SecondaryCareClient(
            SecondaryCareHttpClient httpClient,
            IJsonResponseParser responseParser,
            ILogger<SecondaryCareClient> logger)
        {
            _httpClient = httpClient;
            _responseParser = responseParser;
            _logger = logger;
        }

        public async Task<SecondaryCareResponse<SummaryResponse>> GetSummary(P9UserSession userSession, string accessToken)
            => await Get<SummaryResponse>(SummaryPath, userSession.NhsNumber.RemoveWhiteSpace(), accessToken);

        private async Task<SecondaryCareResponse<TResponse>> Get<TResponse>(string path, string nhsNumber, string accessToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Add(Constants.SecondaryCareConstants.NhsNumberHeader, nhsNumber);

            return await SendRequestAndParseResponse<TResponse>(request);
        }

        private async Task<SecondaryCareResponse<TResponse>> SendRequestAndParseResponse<TResponse>(
            HttpRequestMessage request)
        {
            var responseMessage = await _httpClient.Client.SendAsync(request);
            var response = new SecondaryCareResponse<TResponse>(responseMessage.StatusCode);
            return await response.Parse(responseMessage, _responseParser, _logger);
        }
    }
}