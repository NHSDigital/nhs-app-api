using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Session
{
    internal sealed class TppLogMessagingService
    {
        private readonly ILogger _logger;
        private readonly ITppClient _client;

        public TppLogMessagingService(ILogger<TppLogMessagingService> logger, ITppClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task FetchAndLogAccessInformation(TppUserSession userSession)
        {
            try
            {
                var response = await _client.ListServiceAccessesPost(userSession);
                response.Body?.ServiceAccess?.ForEach(x =>
                {
                    if (string.Equals(x.Description, "Messaging", StringComparison.Ordinal))
                    {
                        _logger.LogInformation($"ODSCode {userSession.OdsCode}  " +
                                               $"PFS messaging enabled: {x.Status} " +
                                               $"with description: {x.StatusDesc}");
                    }
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed request to get list of service accesses, Exception has been thrown");
            }
        }
    }
}