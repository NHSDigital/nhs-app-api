using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CorrelationId.Abstractions;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.APIM;
using NHSOnline.Backend.PfsApi.NHSApim.Models;
using NHSOnline.Backend.Support.ResponseParsers;
using Constants = NHSOnline.Backend.Support.Constants.SecondaryCareConstants;

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
        private readonly ICorrelationContextAccessor _correlationContext;

        public NhsApimClient(
            NhsApimHttpClient nhsApimHttpClient,
            IJsonResponseParser responseParser,
            ILogger<NhsApimClient> logger,
            IApimJwtHelper apimJwtHelper,
            INhsApimConfig config,
            ICorrelationContextAccessor correlationContext)
        {
            _nhsApimHttpClient = nhsApimHttpClient;
            _responseParser = responseParser;
            _logger = logger;
            _apimJwtHelper = apimJwtHelper;
            _config = config;
            _correlationContext = correlationContext;
        }

        public async Task<NhsApimAuthResponse<ApimAccessToken>> GetAuthToken(string nhsLoginIdToken)
            => await PostAuthTokenRequest(AuthPath, nhsLoginIdToken);

        private async Task<NhsApimAuthResponse<ApimAccessToken>> PostAuthTokenRequest(string path, string nhsLoginIdToken)
        {

            var clientAssertionToken = _apimJwtHelper.CreateApimJwt(
                new Uri(_config.BaseUrl + path),
                _config.CertPath,
                _config.CertPassphrase,
                _config.Key,
                _config.Kid);

            var dict = new Dictionary<string, string>
            {
                { "subject_token", nhsLoginIdToken },
                { "client_assertion", clientAssertionToken },
                { "subject_token_type", "urn:ietf:params:oauth:token-type:id_token" },
                { "client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" },
                { "grant_type", "urn:ietf:params:oauth:grant-type:token-exchange" }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, path);
            request.Content = new FormUrlEncodedContent(dict);
            request.Headers.Add(Constants.CorrelationIdHeaderKey, _correlationContext.CorrelationContext.CorrelationId);

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