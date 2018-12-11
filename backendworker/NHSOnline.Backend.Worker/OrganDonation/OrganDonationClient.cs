using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.OrganDonation.Models;
using NHSOnline.Backend.Worker.ResponseParsers;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    public class OrganDonationClient : IOrganDonationClient
    {
        private const string LookupPath = "Registration/_search";

        private const string SessionIdHeaderKey = "X-Session-ID";
        private const string SequenceIdHeaderKey = "X-Sequence-ID";

        private readonly OrganDonationHttpClient _httpClient;
        private readonly IJsonResponseParser _responseParser;
        private readonly ILogger<OrganDonationClient> _logger;

        public OrganDonationClient(OrganDonationHttpClient httpClient,
            IJsonResponseParser responseParser,
            ILogger<OrganDonationClient> logger)
        {
            _httpClient = httpClient;
            _responseParser = responseParser;
            _logger = logger;
        }

        public async Task<OrganDonationResponse<RegistrationLookupResponse>> PostLookup(
            LookupRegistrationRequest request, UserSession userSession)
        {
            return await Post<LookupRegistrationRequest, RegistrationLookupResponse>(request, userSession, LookupPath);
        }

        private async Task<OrganDonationResponse<TResponse>> Post<TRequest, TResponse>(TRequest model,
            UserSession userSession,
            string path)
        {
            var request = BuildRegistrationRequest(HttpMethod.Post, userSession, path);

            var body = JsonConvert.SerializeObject(model);
            request.Content = new StringContent(body, Encoding.UTF8, System.Net.Mime.MediaTypeNames.Application.Json);

            return await SendRequestAndParseResponse<TResponse>(request);
        }

        private static HttpRequestMessage BuildRegistrationRequest(HttpMethod httpMethod, UserSession userSession, string path)
        {
            var request = new HttpRequestMessage(httpMethod, path);
            request.Headers.Add(SessionIdHeaderKey, userSession.OrganDonationSessionId.ToString());
            request.Headers.Add(SequenceIdHeaderKey, Guid.NewGuid().ToString());
            return request;
        }

        private async Task<OrganDonationResponse<TResponse>> SendRequestAndParseResponse<TResponse>(
            HttpRequestMessage request)
        {
            var responseMessage = await _httpClient.Client.SendAsync(request);
            var response = new OrganDonationResponse<TResponse>(responseMessage.StatusCode);
            return await response.Parse(responseMessage, _responseParser, _logger);
        }
    }
}
