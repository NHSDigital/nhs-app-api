using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Api.Client
{
    internal sealed class LoggingHandler : DelegatingHandler
    {
        private static readonly EventId RequestStartEventId = new EventId(100, "RequestStart");
        private static readonly EventId RequestEndEventId = new EventId(101, "RequestEnd");
        private static readonly EventId RequestFailedEventId = new EventId(102, "RequestFailed");

        private static readonly Action<ILogger, HttpMethod, Uri, Exception?> RequestStart
            = LoggerMessage.Define<HttpMethod, Uri>(
                LogLevel.Information,
                RequestStartEventId,
                "Sending HTTP request {HttpMethod} {Uri}");

        private static readonly Action<ILogger, HttpMethod, Uri, HttpStatusCode, Exception?> RequestEnd
            = LoggerMessage.Define<HttpMethod, Uri, HttpStatusCode>(
                LogLevel.Information,
                RequestEndEventId,
                "Received HTTP response {HttpMethod} {Uri} - {StatusCode}");

        private static readonly Action<ILogger, HttpMethod, Uri, Exception?> RequestFailed
            = LoggerMessage.Define<HttpMethod, Uri>(
                LogLevel.Information,
                RequestFailedEventId,
                "Failed HTTP request {HttpMethod} {Uri}");

        private readonly ILogger _logger;

        public LoggingHandler(ILogger logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestStart(_logger, request.Method, request.RequestUri, null);

            try
            {
                var response = await base.SendAsync(request, cancellationToken).ResumeOnThreadPool();

                RequestEnd(_logger, request.Method, request.RequestUri, response.StatusCode, null);

                return response;
            }
            catch (Exception e)
            {
                RequestFailed(_logger, request.Method, request.RequestUri, e);

                throw;
            }
        }
    }
}