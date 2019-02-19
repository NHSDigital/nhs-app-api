using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.ResponseParsers;
using NHSOnline.Backend.Worker.OrganDonation.ApiModels;
using NHSOnline.Backend.Worker.OrganDonation.Models;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    internal class OrganDonationClient : IOrganDonationClient
    {
        private const string LookupPath = "Registration/_search";
        private const string CreatePath = "Registration";
        private const string AllReferencePath = "ReferenceData";

        private const string SessionIdHeaderKey = "X-Session-ID";
        private const string SequenceIdHeaderKey = "X-Sequence-ID";

        private readonly OrganDonationHttpClient _httpClient;
        private readonly IJsonResponseParser _responseParser;
        private readonly ILogger<OrganDonationClient> _logger;
        private readonly JsonSerializerSettings _serializerSettings;

        public OrganDonationClient(OrganDonationHttpClient httpClient,
            IJsonResponseParser responseParser,
            ILogger<OrganDonationClient> logger)
        {
            _httpClient = httpClient;
            _responseParser = responseParser;
            _logger = logger;

            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public async Task<OrganDonationResponse<RegistrationLookupResponse>> PostLookup(
            RegistrationLookupRequest request, UserSession userSession)
        {
            return await Post<RegistrationLookupRequest, RegistrationLookupResponse>
                (request, userSession, LookupPath);
        }

        public async Task<OrganDonationResponse<RegistrationResponse>> PostRegistration(
            RegistrationRequest request, UserSession userSession)
        {
            return await Post<RegistrationRequest, RegistrationResponse>
                (request, userSession, CreatePath);
        }

        public async Task<OrganDonationResponse<ReferenceDataResponse>> GetAllReferenceData()
        {
            return await Get<ReferenceDataResponse>(AllReferencePath);
        }

        public async Task<OrganDonationResponse<RegistrationResponse>> PutUpdate(
            RegistrationRequest request, UserSession userSession)
        {
            return await Put<RegistrationRequest, RegistrationResponse>(
                request, 
                userSession, 
                $"{CreatePath}/{request.Id}");
        }
        private async Task<OrganDonationResponse<TResponse>> Get<TResponse>(string path)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, path);
            return await SendRequestAndParseResponse<TResponse>(request);
        }

        private async Task<OrganDonationResponse<TResponse>> Post<TRequest, TResponse>(
            TRequest model, UserSession userSession, string path)
        {
            var request = BuildRegistrationRequest(HttpMethod.Post, userSession, path, model);

            return await SendRequestAndParseResponse<TResponse>(request);
        }

        private async Task<OrganDonationResponse<TResponse>> Put<TRequest, TResponse>(
            TRequest model, UserSession userSession, string path)
        {
            var request = BuildRegistrationRequest(HttpMethod.Put, userSession, path, model);

            return await SendRequestAndParseResponse<TResponse>(request);
        }

        private HttpRequestMessage BuildRegistrationRequest<TRequest>(HttpMethod httpMethod, UserSession userSession, string path, TRequest model)
        {
            var request = new HttpRequestMessage(httpMethod, path);
            request.Headers.Add(SessionIdHeaderKey, userSession.OrganDonationSessionId.ToString());
            request.Headers.Add(SequenceIdHeaderKey, Guid.NewGuid().ToString());

            var body = JsonConvert.SerializeObject(model, _serializerSettings);
            request.Content = new StringContent(body);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(System.Net.Mime.MediaTypeNames.Application.Json);
            return request;
        }

        private async Task<OrganDonationResponse<TResponse>> SendRequestAndParseResponse<TResponse>(HttpRequestMessage request)
        {
            var responseMessage = await _httpClient.Client.SendAsync(request);
            var response = new OrganDonationResponse<TResponse>(responseMessage.StatusCode);
            return await response.Parse(responseMessage, _responseParser, _logger);
        }
    }
}
