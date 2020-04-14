using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.ResponseParsers;
using NHSOnline.Backend.PfsApi.OrganDonation.ApiModels;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;

namespace NHSOnline.Backend.PfsApi.OrganDonation
{
    internal class OrganDonationClient : IOrganDonationClient
    {
        private const string LookupPath = "Registration/_search";
        private const string RegistrationPath = "Registration";
        private const string AllReferencePath = "ReferenceData";

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
            RegistrationLookupRequest request, P9UserSession userSession)
        {
            return await Post<RegistrationLookupRequest, RegistrationLookupResponse>
                (request, userSession, LookupPath);
        }

        public async Task<OrganDonationResponse<OrganDonationBasicResponse>> PostRegistration(
            RegistrationRequest request, P9UserSession userSession)
        {
            return await Post<RegistrationRequest, OrganDonationBasicResponse>
                (request, userSession, RegistrationPath);
        }

        public async Task<OrganDonationResponse<ReferenceDataResponse>> GetAllReferenceData()
        {
            return await Get<ReferenceDataResponse>(AllReferencePath);
        }

        public async Task<OrganDonationResponse<OrganDonationBasicResponse>> PutUpdate(
            RegistrationRequest request, P9UserSession userSession)
        {
            return await Put<RegistrationRequest, OrganDonationBasicResponse>(
                request,
                userSession,
                $"{RegistrationPath}/{request.Id}");
        }

        public async Task<OrganDonationResponse<OrganDonationBasicResponse>> Delete(
            WithdrawRequest request, P9UserSession userSession)
        {
            return await Delete<WithdrawRequest, OrganDonationBasicResponse>(
                request,
                userSession,
                $"{RegistrationPath}/{request.Id}");
        }

        private async Task<OrganDonationResponse<TResponse>> Get<TResponse>(string path)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, path))
            {
                return await SendRequestAndParseResponse<TResponse>(request);
            }
        }

        private async Task<OrganDonationResponse<TResponse>> Post<TRequest, TResponse>(
            TRequest model, P9UserSession userSession, string path)
        {
            using (var request = BuildRegistrationRequest(HttpMethod.Post, userSession, path, model))
            {
                return await SendRequestAndParseResponse<TResponse>(request);
            }
        }

        private async Task<OrganDonationResponse<TResponse>> Put<TRequest, TResponse>(
            TRequest model, P9UserSession userSession, string path)
        {
            using (var request = BuildRegistrationRequest(HttpMethod.Put, userSession, path, model))
            {
                return await SendRequestAndParseResponse<TResponse>(request);
            }
        }

        private async Task<OrganDonationResponse<TResponse>> Delete<TRequest, TResponse>(
            TRequest model, P9UserSession userSession, string path)
        {
            using (var request = BuildRegistrationRequest(HttpMethod.Delete, userSession, path, model))
            {
                return await SendRequestAndParseResponse<TResponse>(request);
            }
        }

        private HttpRequestMessage BuildRegistrationRequest<TRequest>(HttpMethod httpMethod, P9UserSession userSession, string path, TRequest model)
        {
            var request = new HttpRequestMessage(httpMethod, path);
            request.Headers.Add(Constants.OrganDonationConstants.SessionIdHeaderKey, userSession.OrganDonationSessionId.ToString());
            request.Headers.Add(Constants.OrganDonationConstants.SequenceIdHeaderKey, Guid.NewGuid().ToString());

            var body = JsonConvert.SerializeObject(model, _serializerSettings);
            request.Content = new StringContent(body);
            request.Content.Headers.ContentType =
                MediaTypeHeaderValue.Parse(System.Net.Mime.MediaTypeNames.Application.Json);
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
