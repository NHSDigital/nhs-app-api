using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Nhs.App.Api.Integration.Tests.Services.AccessTokenService;

namespace Nhs.App.Api.Integration.Tests.Services
{
    public class NhsAppApiJwtWrapperClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly IAccessTokenCacheService _accessTokenService;

        public Uri BaseAddress => _httpClient.BaseAddress;

        public NhsAppApiJwtWrapperClient(TestConfiguration testConfiguration, IAccessTokenCacheService accessTokenService)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(testConfiguration.ApplicationUrl)
            };

            _accessTokenService = accessTokenService;
        }

        private async Task<string> GetAccessToken()
        {
            try
            {
                var accessToken = await _accessTokenService.FetchToken();

                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    return accessToken;
                }
                throw new AccessTokenGenerationException("Could not create an access token");
            }
            catch (Exception e)
            {
                throw new AccessTokenGenerationException("Could not create an access token", e);
            }
        }

        public async Task<HttpResponseMessage> PostAsync(
            string requestUri, HttpContent content, string correlationId = null, string accessToken = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = content };

            return await SendAsync(request, correlationId, accessToken);
        }

        public async Task<HttpResponseMessage> GetAsync(
            string requestUri, string correlationId = null, string accessToken = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            return await SendAsync(request, correlationId, accessToken);
        }

        private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, string correlationId = null, string accessToken = null)
        {
            accessToken ??= await GetAccessToken();

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            if (correlationId != null)
            {
                request.Headers.Add("X-Correlation-ID", correlationId);
            }

            return await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
        }

        public void AddHeader(string key, string value)
        {
            _httpClient.DefaultRequestHeaders.Add(key, value);
        }

        public void RemoveHeader(string key)
        {
            _httpClient.DefaultRequestHeaders.Remove(key);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
