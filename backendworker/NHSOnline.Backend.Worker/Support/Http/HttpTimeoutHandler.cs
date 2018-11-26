using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Worker.Settings;

namespace NHSOnline.Backend.Worker.Support.Http
{
    public class HttpTimeoutHandler<TRequestIdentifier> : DelegatingHandler where TRequestIdentifier : IHttpRequestIdentifier
    {
        private readonly ILogger<HttpTimeoutHandler<TRequestIdentifier>> _logger;
        private readonly IOptions<ConfigurationSettings> _settings;
        private readonly IHttpRequestIdentifier _requestIdentifier;
        private readonly TimeSpan _timeOut;

        public HttpTimeoutHandler(ILogger<HttpTimeoutHandler<TRequestIdentifier>> logger, IOptions<ConfigurationSettings> settings
            , TRequestIdentifier requestIdentifier)
        {
            _logger = logger;
            _settings = settings;
            _requestIdentifier = requestIdentifier;
            _timeOut = TimeSpan.FromSeconds(_settings.Value.DefaultHttpTimeoutSeconds);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            using (var cts = GetCancellationTokenSource(cancellationToken))
            {
                try
                {
                    return await base.SendAsync(request, cts?.Token ?? cancellationToken);
                    
                }
                catch(OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    var requestIdentity = _requestIdentifier.Identify(request);
                    _logger.LogError($"Upstream request timeout: {requestIdentity}");
                    throw new TimeoutException(_requestIdentifier.Identify(request).ToString());
                }
            }
        }
        
        private CancellationTokenSource GetCancellationTokenSource(CancellationToken cancellationToken)
        {
            if (_timeOut == Timeout.InfiniteTimeSpan)
            {
                return null;
            }

            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(_timeOut);
            return cts;
        }
    }
}