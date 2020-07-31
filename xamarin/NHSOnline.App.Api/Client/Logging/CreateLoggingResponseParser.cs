using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Api.Client;
using NHSOnline.App.Api.Client.Errors;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Api.Logging
{
    internal sealed class CreateLoggingResponseParser: IApiClientResponseParser<ApiCreateLogResult>
    {

        private readonly ILogger _logger;

        public CreateLoggingResponseParser(
            ILogger<CreateLoggingResponseParser> logger)
        {
            _logger = logger;
        }

        public async Task<ApiCreateLogResult> Parse(HttpResponseMessage httpResponseMessage)
        {
            var handler = httpResponseMessage.StatusCode switch
            {
                HttpStatusCode.OK => HandleCreated(),
                _ => HandleUnknownStatusCode(httpResponseMessage)
            };

            return await handler.ResumeOnThreadPool();
        }

        private Task<ApiCreateLogResult> HandleCreated()
        {
            _logger.LogInformation("Successfully logged message");

            return Task.FromResult<ApiCreateLogResult>(new ApiCreateLogResult.Created());
        }

        private Task<ApiCreateLogResult> HandleUnknownStatusCode(HttpResponseMessage httpResponseMessage)
        {
            _logger.LogWarning(
                "Log message returned unexpected HTTP status {UnexpectedHttpStatue}",
                httpResponseMessage.StatusCode);
            return Task.FromResult<ApiCreateLogResult>(new ApiCreateLogResult.Failure());
        }
    }
}