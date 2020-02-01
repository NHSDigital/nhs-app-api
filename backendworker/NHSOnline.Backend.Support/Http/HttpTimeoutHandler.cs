using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.AspNet.Filters;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.Support.Http
{
    public class HttpTimeoutHandler<TRequestIdentifier> : DelegatingHandler where TRequestIdentifier : IHttpRequestIdentifier
    {
        private readonly ILogger<HttpTimeoutHandler<TRequestIdentifier>> _logger;
        private readonly IHttpRequestIdentifier _requestIdentifier;
        private readonly TimeSpan _defaultTimeout;

        public HttpTimeoutHandler(
            ILogger<HttpTimeoutHandler<TRequestIdentifier>> logger, IHttpTimeoutConfigurationSettings settings
            , TRequestIdentifier requestIdentifier)
        {
            _logger = logger;
            _requestIdentifier = requestIdentifier;
            _defaultTimeout = TimeSpan.FromSeconds(settings.DefaultHttpTimeoutSeconds);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var timeOut = request.Properties.TryGetValue(HttpRequestConstants.CustomTimeout, out var value)
                ? TimeSpan.FromSeconds((int) value)
                : _defaultTimeout;

            using (var cts = GetCancellationTokenSource(timeOut, cancellationToken))
            {
                try
                {
                    return await base.SendAsync(request, cts?.Token ?? cancellationToken);
                    
                }
                catch(OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    var requestIdentity = _requestIdentifier.Identify(request);
                    _logger.LogError($"Upstream request timeout: {requestIdentity}");
                    var message = _requestIdentifier.Identify(request).ToString();
                    var timeoutType = new NhsTimeoutException(message)
                    {
                        SourceApi = requestIdentity.SourceApi
                    };
                    
                    throw timeoutType;
                }
            }
        }
        
        private static CancellationTokenSource GetCancellationTokenSource(TimeSpan timeOut, CancellationToken cancellationToken)
        {
            if (timeOut == Timeout.InfiniteTimeSpan)
            {
                return null;
            }

            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(timeOut);
            return cts;
        }
    }
}