using System.Net.Http;
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

        internal async Task<HttpResponseMessage> Get(string endpoint, string accessToken)
            => await Send(HttpMethod.Post, endpoint, accessToken, null).ResumeOnThreadPool();

        internal async Task<HttpResponseMessage> Post(StringContent content, string endpoint, string accessToken)
            => await Send(HttpMethod.Post, endpoint, accessToken, content).ResumeOnThreadPool();

        private async Task<HttpResponseMessage> Send(HttpMethod httpMethod, string endpoint, string accessToken, HttpContent? content)
        {
            using var requestMessage = new HttpRequestMessage(httpMethod, endpoint);
            requestMessage.Headers.Add("Authorization", $"Bearer {accessToken}");
            if (content != null)
            {
                requestMessage.Content = content;
            }
            return await _httpClient.SendAsync(requestMessage).ResumeOnThreadPool();
        }
    }
}