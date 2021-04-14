using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using NHSOnline.App.Threading;

namespace NHSOnline.App.NhsLogin.Fido
{
    internal sealed class UafHttpClient
    {
        private readonly HttpClient _httpClient;

        public UafHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        internal async Task<HttpResponseMessage> Get(string endpoint)
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, endpoint);

            return await _httpClient.SendAsync(requestMessage).ResumeOnThreadPool();
        }

        internal async Task<HttpResponseMessage> Get(string endpoint, string accessToken)
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, endpoint);
            AddAuthorizationHeader(requestMessage, accessToken);

            return await _httpClient.SendAsync(requestMessage).ResumeOnThreadPool();
        }

        internal async Task<HttpResponseMessage> Post(StringContent content, string endpoint, string accessToken)
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint) {Content = content};
            AddAuthorizationHeader(requestMessage, accessToken);

            return await _httpClient.SendAsync(requestMessage).ResumeOnThreadPool();
        }

        private static void AddAuthorizationHeader(HttpRequestMessage message, string accessToken)
        {
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }
}