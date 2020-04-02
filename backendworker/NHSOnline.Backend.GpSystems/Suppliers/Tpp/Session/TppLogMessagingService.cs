using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Services;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Session
{
    internal sealed class TppLogMessagingService
    {
        private readonly ILogger _logger;
        private readonly ITppClientRequest<TppUserSession, ListServiceAccessesReply> _listServiceAccesses;

        public TppLogMessagingService(
          ILogger<TppLogMessagingService> logger,
          ITppClientRequest<TppUserSession, ListServiceAccessesReply> listServiceAccesses)
        {
            _logger = logger;
            _listServiceAccesses = listServiceAccesses;
        }

        public async Task FetchAndLogAccessInformation(TppUserSession userSession)
        {
            try
            {
                var response = await _listServiceAccesses.Post(userSession);
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