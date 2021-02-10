using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Nhs.App.Api.Integration.Tests.Services
{
    public class NhsAppApiApiKeyWrapperClient : IDisposable
    {
        private readonly TestConfiguration _testConfiguration;
        private readonly HttpClient _httpClient;

        public HttpRequestHeaders DefaultRequestHeaders => _httpClient.DefaultRequestHeaders;

        public NhsAppApiApiKeyWrapperClient(TestConfiguration testConfiguration)
        {
            _testConfiguration = testConfiguration;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(testConfiguration.ApplicationUrl)
            };
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, string apiKey = null)
        {
            apiKey ??= _testConfiguration.ApigeeApiKey;

            var request = new HttpRequestMessage(HttpMethod.Post, requestUri) {Content = content};

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("X-Correlation-ID", Guid.NewGuid().ToString());
            request.Headers.Add("X-Api-Key", apiKey);

            return await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
