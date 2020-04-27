using System;
using System.Collections.Generic;
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
        private const string SigningKeysPath = ".well-known/jwks.json";
        private const string UserInfoPath = "userinfo";

        private readonly CitizenIdHttpClient _httpClient;
        private readonly ICitizenIdConfig _config;
        private readonly IJsonResponseParser _responseParser;
        private readonly ICitizenIdJwtHelper _citizenIdJwtHelper;

        public CitizenIdClient(
            CitizenIdHttpClient httpClient,
            ICitizenIdConfig config,
            ILogger<CitizenIdClient> logger,
            IJsonResponseParser responseParser,
            ICitizenIdJwtHelper citizenIdJwtHelper)
        {
            _logger = logger;
            _httpClient = httpClient;

            _responseParser = responseParser;
            _config = config;
            _citizenIdJwtHelper = citizenIdJwtHelper;
        }

        public async Task<CitizenIdApiObjectResponse<Token>> ExchangeAuthToken(
            string authCode,
            string codeVerifier,
            Uri redirectUrl)
        {
            try
            {
                _logger.LogEnter();

                using (var request = new HttpRequestMessage(HttpMethod.Post, _config.TokenPath))
                {
                    var token = _citizenIdJwtHelper.CreateClientAuthJwt();

                    var dict = new Dictionary<string, string>
                    {
                        { "grant_type", "authorization_code" },
                        { "code", authCode },
                        { "redirect_uri", redirectUrl.ToString() },
                        { "code_verifier", codeVerifier },
                        { "code_challenge_method", "S256" },
                        { "client_assertion", token },
                        { "client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" }
                    };

                    request.Content = new FormUrlEncodedContent(dict);

                    var response = await SendRequestAndParseResponse<Token>(request);
                    return response;
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<CitizenIdApiObjectResponse<UserInfo>> GetUserInfo(string accessToken)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, UserInfoPath))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await SendRequestAndParseResponse<UserInfo>(request);
                return response;
            }
        }

        public async Task<CitizenIdApiObjectResponse<Token>> RefreshAccessToken(string refreshToken)
        {
            try
            {
                _logger.LogEnter();

                using (var request = new HttpRequestMessage(HttpMethod.Post, _config.TokenPath))
                {
                    var token = _citizenIdJwtHelper.CreateClientAuthJwt();

                    var dict = new Dictionary<string, string>
                    {
                        { "grant_type", "refresh_token" },
                        { "client_assertion", token },
                        { "client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" },
                        { "refresh_token", refreshToken}
                    };

                    request.Content = new FormUrlEncodedContent(dict);

                    var response = await SendRequestAndParseResponse<Token>(request);
                    return response;
                }
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<CitizenIdApiObjectResponse<JsonWebKeySet>> GetSigningKeys()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, SigningKeysPath))
            {
                var response = await SendRequestAndParseResponse<JsonWebKeySet>(request);
                return response;
            }
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

            response.Body = _responseParser.ParseBody<TResponse>(stringResponse);
            response.ErrorResponse = _responseParser.ParseError<ErrorResponse>(stringResponse, responseMessage);

            return response;
        }
    }
}