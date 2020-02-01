using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientRequestExecutor
    {
        private readonly ILogger<TppClientRequestExecutor> _logger;
        private readonly Func<TppClientRequestBuilder> _requestBuilderFactory;
        private readonly ITppClientRequestSender _requestSender;

        public TppClientRequestExecutor(
            ILogger<TppClientRequestExecutor> logger,
            Func<TppClientRequestBuilder> requestBuilderFactory,
            ITppClientRequestSender requestSender)
        {
            _logger = logger;
            _requestBuilderFactory = requestBuilderFactory;
            _requestSender = requestSender;
        }

        public async Task<TppApiObjectResponse<TReply>> Post<TReply>(
            Action<TppClientRequestBuilder> buildRequest)
        {
            using (var requestBuilder = _requestBuilderFactory())
            {
                buildRequest(requestBuilder);

                _logger.LogInformation($"Sending TPP REQUEST TYPE: {requestBuilder.RequestType} UUID: {requestBuilder.Uuid}");

                return await _requestSender.SendRequestAndParseResponse<TReply>(requestBuilder.Build());
            }
        }
    }
}