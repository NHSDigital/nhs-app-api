using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NHSOnline.Backend.Worker.CitizenId.Models;
using NHSOnline.Backend.Support.ResponseParsers;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.Worker.CitizenId
{
    public interface ICitizenIdClient
    {
        [SuppressMessage("Microsoft.Design", "CA1054", Justification = "Uris are not serializable")]
        Task<CitizenIdClient.CitizenIdApiObjectResponse<Token>> ExchangeAuthToken(string authCode, string codeVerifier, string redirectUrl);
        Task<CitizenIdClient.CitizenIdApiObjectResponse<JsonWebKeySet>>GetSigningKeys();
    }

    public class CitizenIdClient : ICitizenIdClient
    {
        private readonly ILogger<CitizenIdClient> _logger;
        private const string TokenPath = "token";
        private const string SigningKeysPath = ".well-known/jwks.json";

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
        public async Task<CitizenIdApiObjectResponse<Token>> ExchangeAuthToken(string authCode, string codeVerifier, string redirectUrl)
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
            