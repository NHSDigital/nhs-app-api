using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.CitizenId.Models;

namespace NHSOnline.Backend.Worker.CitizenId
{
    public interface ICitizenIdClient
    {
        Task<CitizenIdClient.CitizenIdApiObjectResponse<Token>> ExchangeAuthToken(string authCode, string codeVerifier);
        Task<CitizenIdClient.CitizenIdApiObjectResponse<UserInfo>> GetUserInfo(string bearerToken);
    }

    public class CitizenIdClient : ICitizenIdClient
    {
        private readonly ILogger<CitizenIdClient> _logger;
        private const string TokenPath = "token";
        private const string UserInfoPath = "userinfo";

        private readonly HttpClient _httpClient;
        private readonly string _redirectUri;
        private readonly string _clientId;
        private readonly string _basicAuthCredentials;

        public CitizenIdClient(
            IHttpClientFactory httpClientFactory, 
            ICitizenIdConfig config, 
            ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CitizenIdClient>();
            _httpClient = httpClientFactory.GetClient(HttpClientName.CitizenIdApiClient);
            _httpClient.BaseAddress = config.CitizenIdApiBaseUrl;

            _redirectUri = new Uri(config.NhsWebAppBaseUrl, "auth-return").ToString();
            _clientId = config.ClientId;
            _basicAuthCredentials = Convert.ToBase64String(
                System.Text.Encoding.ASCII.GetBytes($"{config.ClientId}:{config.ClientSecret}"));
        }

        public async Task<CitizenIdApiObjectResponse<Token>> ExchangeAuthToken(string authCode, string codeVerifier)
        {
            _logger.LogDebug("Starting ExchangeAuthToken");
            var dict = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", authCode },
                { "redirect_uri", _redirectUri },
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

        public async Task<CitizenIdApiObjectResponse<UserInfo>> GetUserInfo(string bearerToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, UserInfoPath);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await SendRequestAndParseResponse<UserInfo>(request);
            return response;
        }

        private async Task<CitizenIdApiObjectResponse<TResponse>> SendRequestAndParseResponse<TResponse>(
            HttpRequestMessage request)
        {
            var message = "Sending request to:" +
                          $"   RequestUri: {request.RequestUri}";
            
            _logger.LogDebug(message);
            var responseMessage = await _httpClient.SendAsync(request);
            var response = new CitizenIdApiObjectResponse<TResponse>(responseMessage.StatusCode);

            var stringResponse = responseMessage.Content != null
                ? await responseMessage.Content.ReadAsStringAsync()
                : null;
            
            if (string.IsNullOrEmpty(stringResponse)) return response;
            
            response.Body = stringResponse.ParseBody<TResponse>(responseMessage);
            response.ErrorResponse = stringResponse.ParseError<ErrorResponse>(responseMessage);

            return response;
        }
        
        public class CitizenIdApiResponse
        {
            protected CitizenIdApiResponse(HttpStatusCode statusCode)
            {
                StatusCode = statusCode;
            }

            public HttpStatusCode StatusCode { get; set; }
            public ErrorResponse ErrorResponse { get; set; }
            public bool HasSuccessStatusCode => (int)StatusCode >= 200 && (int)StatusCode <= 299;
        }

        public class CitizenIdApiObjectResponse<TBody> : CitizenIdApiResponse
        {
            public CitizenIdApiObjectResponse(HttpStatusCode statusCode) : base(statusCode)
            {
            }

            public TBody Body { get; set; }
        }
    }
}