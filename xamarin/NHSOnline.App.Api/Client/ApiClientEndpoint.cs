using System.Net.Http;
using System.Threading.Tasks;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Api.Client
{
    internal sealed class ApiClientEndpoint<TRequest, TResult> : IApiClientEndpoint<TRequest, TResult>
    {
        private readonly ApiHttpClient _apiHttpClient;
        private readonly IApiClientRequestMessageBuilder<TRequest> _requestMessageBuilder;
        private readonly IApiClientResponseParser<TResult> _responseMessageParser;

        public ApiClientEndpoint(
            ApiHttpClient apiHttpClient,
            IApiClientRequestMessageBuilder<TRequest> requestMessageBuilder,
            IApiClientResponseParser<TResult> responseMessageParser)
        {
            _apiHttpClient = apiHttpClient;
            _requestMessageBuilder = requestMessageBuilder;
            _responseMessageParser = responseMessageParser;
        }

        public async Task<TResult> Call(TRequest request)
        {
            using var requestMessage = new HttpRequestMessage();
            await _requestMessageBuilder.Build(request, requestMessage).ResumeOnThreadPool();

            using var responseMessage = await _apiHttpClient.Send(requestMessage).ResumeOnThreadPool();

            return await _responseMessageParser.Parse(responseMessage).ResumeOnThreadPool();
        }
    }
}