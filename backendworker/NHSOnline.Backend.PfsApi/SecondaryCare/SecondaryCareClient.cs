using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public class SecondaryCareClient : ISecondaryCareClient
    {
        private const string SummaryPath = "summary/$evaluate";

        private readonly SecondaryCareHttpClient _httpClient;
        private readonly ILogger<SecondaryCareClient> _logger;

        public SecondaryCareClient(
            SecondaryCareHttpClient httpClient,
            ILogger<SecondaryCareClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<SecondaryCareResponse> GetSummary(P9UserSession userSession, string accessToken)
            => await Get(SummaryPath, userSession.NhsNumber.RemoveWhiteSpace(), accessToken);

        private async Task<SecondaryCareResponse> Get(string path, string nhsNumber, string accessToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Add(Constants.SecondaryCareConstants.NhsNumberHeader, nhsNumber);

            return await SendRequestAndParseResponse(request);
        }

        private async Task<SecondaryCareResponse> SendRequestAndParseResponse(HttpRequestMessage request)
        {
            var responseMessage = await _httpClient.Client.SendAsync(request);
            var response = new SecondaryCareResponse(responseMessage.StatusCode);
            return await response.Parse(responseMessage, _logger);
        }
    }
}