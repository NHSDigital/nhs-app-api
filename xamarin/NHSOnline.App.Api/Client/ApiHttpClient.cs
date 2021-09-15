using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Api.Client
{
    internal sealed class ApiHttpClient
    {
        private readonly HttpClient _httpClient;

        public Uri BaseUri => _httpClient.BaseAddress;

        public ApiHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        internal async Task<HttpResponseMessage> Send(HttpRequestMessage request, CancellationToken token)
            => await _httpClient.SendAsync(request, token).ResumeOnThreadPool();
    }
}