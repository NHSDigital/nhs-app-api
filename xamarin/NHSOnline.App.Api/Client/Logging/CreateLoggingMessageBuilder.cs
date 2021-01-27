using System;
using System.Net.Http;
using System.Threading.Tasks;
using NHSOnline.App.Api.Client;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Api.Logging
{
    internal sealed class CreateLoggingMessageBuilder: IApiClientRequestMessageBuilder<ApiCreateLogRequest>
    {
        private readonly JsonRequestContentSerialiser _serializer;

        public CreateLoggingMessageBuilder(JsonRequestContentSerialiser serializer)
        {
            _serializer = serializer;
        }

        public async Task Build(ApiCreateLogRequest request, HttpRequestMessage httpRequestMessage)
        {
            httpRequestMessage.Method = HttpMethod.Post;
            httpRequestMessage.RequestUri = new Uri("/v1/api/log");
            await _serializer.SetContent(httpRequestMessage, request.CreateModel()).ResumeOnThreadPool();
        }
    }
}