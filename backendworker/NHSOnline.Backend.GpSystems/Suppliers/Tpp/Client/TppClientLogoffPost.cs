using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientLogoffPost
        : ITppClientRequest<TppUserSession, LogoffReply>
    {
        private readonly ILogger<TppClientLogoffPost> _logger;
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientLogoffPost(ILogger<TppClientLogoffPost> logger, TppClientRequestExecutor requestExecutor)
        {
            _logger = logger;
            _requestExecutor = requestExecutor;
        }

        public async Task<TppApiObjectResponse<LogoffReply>> Post(TppUserSession tppUserSession)
        {
            _logger.LogEnter();

            try
            {
                var request = new Logoff();

                return await _requestExecutor.Post<LogoffReply>(
                    requestBuilder => requestBuilder.Model(request));
            }
            finally
            {
                _logger.LogExit();
            }

        }
    }
}