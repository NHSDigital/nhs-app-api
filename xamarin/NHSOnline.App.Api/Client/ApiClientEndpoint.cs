using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Config;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Api.Client
{
    internal sealed class ApiClientEndpoint<TRequest, TResult> : IApiClientEndpoint<TRequest, TResult>
    {
        private readonly ILogger _logger;
        private readonly INhsAppApiConfiguration _config;
        private readonly IApiClientRequestMessageBuilder<TRequest> _requestMessageBuilder;
        private readonly IApiClientResponseParser<TResult> _responseMessageParser;

        public ApiClientEndpoint(
            ILogger<ApiClientEndpoint<TRequest, TResult>> logger,
            IApiClientRequestMessageBuilder<TRequest> requestMessageBuilder,
            IApiClientResponseParser<TResult> responseMessageParser,
            INhsAppApiConfiguration config)
        {
            _logger = logger;
            _config = config;
            _requestMessageBuilder = requestMessageBuilder;
            _responseMessageParser = responseMessageParser;
        }

        public async Task<TResult> Call(TRequest request)
        {
            // TODO: Replace with IHttpClientFactory
            var cookies = new CookieContainer();
            using var handler = new HttpClientHandler {CookieContainer = cookies};
            using var loggingHandler = new LoggingHandler(_logger) {InnerHandler = handler};
            using var client = new HttpClient(loggingHandler)
            {
                BaseAddress = _config.BaseAddress
            };

            using var requestMessage = new HttpRequestMessage();
            await _requestMessageBuilder.Build(request, requestMessage, cookies).ResumeOnThreadPool();

            using var responseMessage = await client.SendAsync(requestMessage).ResumeOnThreadPool();

            return await _responseMessageParser.Parse(responseMessage, cookies).ResumeOnThreadPool();
        }
    }
}