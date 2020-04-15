using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientLogoffPost
        : ITppClientRequest<TppRequestParameters, LogoffReply>
    {
        private readonly ILogger<TppClientLogoffPost> _logger;
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientLogoffPost(ILogger<TppClientLogoffPost> logger, TppClientRequestExecutor requestExecutor)
        {
            _logger = logger;
            _requestExecutor = requestExecutor;
        }

        public async Task<TppApiObjectResponse<LogoffReply>> Post(TppRequestParameters tppRequestParameters)
        {
            _logger.LogEnter();

            try
            {
                var request = new Logoff();

                return await _requestExecutor.Post<LogoffReply>(
                    requestBuilder => requestBuilder.Model(request).Suid(tppRequestParameters.Suid));
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}