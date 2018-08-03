using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.CitizenId.Models;
using NHSOnline.Backend.Worker.ResponseParsers;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.CitizenId
{
    public interface ICitizenIdClient
    {
        [SuppressMessage("Microsoft.Design", "CA1054", Justification = "Uris are not serializable")]
        Task<CitizenIdClient.CitizenIdApiObjectResponse<Token>> ExchangeAuthToken(string authCode, string codeVerifier, string redirectUrl);
        [SuppressMessage("Microsoft.Design", "CA1054", Justification = "Uris are not serializable")]
        Task<CitizenIdClient.CitizenIdApiObjectResponse<UserInfo>> GetUserInfo(string bearerToken);
    }

    public class CitizenIdClient : ICitizenIdClient
    {
        private readonly ILogger<CitizenIdClient> _logger;
        private const string TokenPath = "token";
        private const string UserInfoPath = "userinfo";

        private readonly CitizenIdHttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _basicAuthCredentials;
        private readonly IJsonResponseParser _responseParser;

        public CitizenIdClient(
            CitizenIdHttpClient httpClient, 
            ICitizenIdConfig config, 
            ILoggerFactory loggerFactory,
            IJsonResponseParser responseParser)
        {
            _logger = loggerFactory.CreateLogger<CitizenIdClient>();
            _httpClient = httpClient;
            
            _responseParser = responseParser;
            _clientId = config.ClientId;
            _basicAuthCredentials = Convert.ToBase64String(
                System.Text.Encoding.ASCII.GetBytes($"{config.ClientId}:{config.ClientSecret}"));
        }

        [SuppressMessage("Microsoft.Design", "CA1054", Justification = "Uris are not serializable")]
        public async Task<CitizenIdApiObjectResponse<Token>> ExchangeAuthToken(string authCode, string codeVerifier, string redirectUrl)
        {
            try
            {
                _logger.LogEnter(nameof(ExchangeAuthToken));
                
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
                _logger.LogExit(nameof(ExchangeAuthToken));
            }
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
            var responseMessage = await _httpClient.Client.SendAsync(request);

            var response = new CitizenIdApiObjectResponse<TResponse>(responseMessage.StatusCode);

            var stringResponse = responseMessage.Content != null
                ? await responseMessage.Content.ReadAsStringAsync()
                : null;
            
            if (string.IsNullOrEmpty(stringResponse)) return response;
            
            response.Body = _responseParser.ParseBody<TResponse>(stringResponse, responseMessage);
            response.ErrorResponse = _responseParser.ParseError<ErrorResponse>(stringResponse, responseMessage);

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
            