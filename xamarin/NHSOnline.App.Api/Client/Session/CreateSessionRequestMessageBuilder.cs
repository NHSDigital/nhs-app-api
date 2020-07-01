using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Api.Client.Session
{
    internal sealed class CreateSessionRequestMessageBuilder: IApiClientRequestMessageBuilder<ApiCreateSessionRequest>
    {
        private readonly JsonRequestContentSerialiser _serializer;

        public CreateSessionRequestMessageBuilder(JsonRequestContentSerialiser serializer)
        {
            _serializer = serializer;
        }

        public async Task Build(ApiCreateSessionRequest request, HttpRequestMessage httpRequestMessage, CookieContainer cookies)
        {
            httpRequestMessage.Method = HttpMethod.Post;
            httpRequestMessage.RequestUri = new Uri("/v1/session");
            await _serializer.SetContent(httpRequestMessage, request.CreateModel()).ResumeOnThreadPool();
        }
    }
}