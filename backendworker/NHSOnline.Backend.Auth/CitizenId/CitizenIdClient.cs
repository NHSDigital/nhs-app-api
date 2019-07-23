using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.ResponseParsers;

namespace NHSOnline.Backend.Auth.CitizenId
{
    public class CitizenIdClient : ICitizenIdClient
    {
        private readonly ILogger<CitizenIdClient> _logger;
        private const string TokenPath = "token";
        private const string SigningKeysPath = ".well-known/jwks.json";
        private const string UserInfoPath = "userinfo";

        private readonly CitizenIdHttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _basicAuthCredentials;
        private readonly IJsonResponseParser _responseParser;

        public CitizenIdClient(
            CitizenIdHttpClient httpClient,
            ICitizenIdConfig config,
            ILogger<CitizenIdClient> logger,
            IJsonResponseParser responseParser)
        {
            _logger = logger;
            _httpClient = httpClient;

            _responseParser = responseParser;
            _clientId = config.ClientId;
            _basicAuthCredentials = Convert.ToBase64String(
                System.Text.Encoding.ASCII.GetBytes($"{config.ClientId}:{config.ClientSecret}"));
        }

        [SuppressMessage("Microsoft.Design", "CA1054", Justification = "Uris are not serializable")]
        public async Task<CitizenIdApiObjectResponse<Token>> ExchangeAuthToken(string authCode, string codeVerifier,
            string redirectUrl)
        {
            try
            {
                _logger.LogEnter();

                var dict = new Dictionary<string, string>
                {
                    { "grant_type", "authorization_code" },
                    { "code", authCode },
                    { "redirect_uri", redirectUrl },
                    { "code_verifier", codeVerifier },
                    { "client_id", _clientId },
                    { "code_challenge_method", "S256" }
                };

                var request = new HttpRequestMessage(HttpMethod.Post, TokenPath)
                {
                    Content = new FormUrlEncodedContent(dict)
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", _basicAuthCredentials);

                var response = await SendRequestAndParseResponse<Token>(request);
                return response;
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<CitizenIdApiObjectResponse<UserInfo>> GetUserInfo(string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, UserInfoPath);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await SendRequestAndParseResponse<UserInfo>(request);
            return response;
        }
        
        public async Task<CitizenIdApiObjectResponse<JsonWebKeySet>> GetSigningKeys()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, SigningKeysPath);

            var response = await SendRequestAndParseResponse<JsonWebKeySet>(request);
            return response;
        }

        private async Task<CitizenIdApiObjectResponse<TResponse>> SendRequestAndParseResponse<TResponse>(
            HttpRequestMessage request)
        {
            var responseMessage = await _httpClient.Client.SendAsync(request);

            var response = new CitizenIdApiObjectResponse<TResponse>(responseMessage.StatusCode);

            var stringResponse = responseMessage.Content != null
                ? await responseMessage.Content.ReadAsStringAsync()
                : null;

            if (string.IsNullOrEmpty(stringResponse))
            {
                return response;
            }

            response.Body = _responseParser.ParseBody<TResponse>(stringResponse, responseMessage);
            response.ErrorResponse = _responseParser.ParseError<ErrorResponse>(stringResponse, responseMessage);

            return response;
        }
    }
}