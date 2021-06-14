using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NHSOnline.App.Api.Client.Configuration
{
    internal sealed class GetConfigurationRequestMessageBuilder : IApiClientRequestMessageBuilder<ApiConfigurationRequest>
    {
        public Task Build(ApiConfigurationRequest request, HttpRequestMessage httpRequestMessage)
        {
            httpRequestMessage.Method = HttpMethod.Get;
            httpRequestMessage.RequestUri = new Uri("/v2/configuration");
            return Task.FromResult(httpRequestMessage);
        }
    }
}