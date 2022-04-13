using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.APIM;
using NHSOnline.Backend.PfsApi.NHSApim.Models;
using NHSOnline.Backend.PfsApi.SecondaryCare;
using NHSOnline.Backend.Support.ResponseParsers;

namespace NHSOnline.Backend.PfsApi.NHSApim
{
    public class NhsApimClient : INhsApimClient
    {
        private const string AuthPath = "oauth2/token";

        private readonly NhsApimHttpClient _nhsApimHttpClient;
        private readonly IJsonResponseParser _responseParser;
        private readonly ILogger<NhsApimClient> _logger;
        private readonly IApimJwtHelper _apimJwtHelper;
        private readonly INhsApimConfig _config;

        public NhsApimClient(
            NhsApimHttpClient nhsApimHttpClient,
            IJsonResponseParser responseParser,
            ILogger<NhsApimClient> logger,
            IApimJwtHelper apimJwtHelper,
            INhsApimConfig config)
        {
            _nhsApimHttpClient = nhsApimHttpClient;
            _responseParser = responseParser;
            _logger = logger;
            _apimJwtHelper = apimJwtHelper;
            _config = config;
        }

        public async Task<NhsApimAuthResponse<ApimAccessToken>> GetAuthToken()
            => await PostAuthTokenRequest(AuthPath);

        private async Task<NhsApimAuthResponse<ApimAccessToken>> PostAuthTokenRequest(string path)
        {

            var token = _apimJwtHelper.CreateApimJwt(
                new Uri(_config.BaseUrl + path),
                _config.CertPath,
                _config.CertPassphrase,
                _config.Key,
                _config.Kid);

            var dict = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" },
                { "client_assertion", token },
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, path);
            request.Content = new FormUrlEncodedContent(dict);

            return await SendRequestAndParseResponse(request);
        }

        private async Task<NhsApimAuthResponse<ApimAccessToken>> SendRequestAndParseResponse(
            HttpRequestMessage request)
        {
            var responseMessage = await _nhsApimHttpClient.Client.SendAsync(request);
            var response = new NhsApimAuthResponse<ApimAccessToken>(responseMessage.StatusCode);
            return await response.Parse(responseMessage, _responseParser, _logger);
        }
    }
}