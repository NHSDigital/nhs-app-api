using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Session
{
    public class EmisSessionExtendService : ISessionExtendService
    {

        private readonly IEmisClient _emisClient;
        private readonly ILogger<EmisSessionExtendService> _logger;

        private const string StandardErrorMessage = "Failed request to extend Emis user session, while attempting to extend session.";

        public EmisSessionExtendService(IEmisClient emisClient,
            ILogger<EmisSessionExtendService> logger)
        {
            _emisClient = emisClient;
            _logger = logger;
        }

        public async Task<SessionExtendResult> Extend(UserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                var emisUserSession = (EmisUserSession) userSession;
                var headerParams = new EmisHeaderParameters(emisUserSession);
                var response = await _emisClient.PracticeSettingsGet(headerParams, emisUserSession.OdsCode);

                if (response.HasSuccessStatusCode)
                {
                    return new SessionExtendResult.SuccessfullyExtended();
                }
                _logger.LogError($"{StandardErrorMessage} {response.ErrorForLogging()}");
                return new SessionExtendResult.SupplierSystemUnavailable();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, $"{StandardErrorMessage} HttpRequestException has been thrown.");
                return new SessionExtendResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}