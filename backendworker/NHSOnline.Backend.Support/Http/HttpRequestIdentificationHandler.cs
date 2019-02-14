using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support.Http
{
    public class HttpRequestIdentificationHandler<TRequestIdentifier> : DelegatingHandler
        where TRequestIdentifier : IHttpRequestIdentifier
    {
        private readonly ILogger<HttpRequestIdentificationHandler<TRequestIdentifier>> _logger;
        private readonly IHttpRequestIdentifier _requestIdentifier;

        public HttpRequestIdentificationHandler(ILogger<HttpRequestIdentificationHandler<TRequestIdentifier>> logger,
            TRequestIdentifier requestIdentifier)
        {
            _logger = logger;
            _requestIdentifier = requestIdentifier;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var identifier = _requestIdentifier.Identify(request);
            
            using(_logger.BeginScope($"Provider={identifier.Provider} UpStreamIdentifier={identifier.Identifier}"))
            {
                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}