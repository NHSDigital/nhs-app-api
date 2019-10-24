using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Session
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

        public async Task<SessionExtendResult> Extend(GpUserSession gpUserSession)
        {
            try
            {
                _logger.LogEnter();

                var emisUserSession = (EmisUserSession)gpUserSession;
                var response = await _emisClient.DemographicsGet(
                    new EmisRequestParameters
                    {
                        UserPatientLinkToken = emisUserSession.UserPatientLinkToken,
                        SessionId = emisUserSession.SessionId,
                        EndUserSessionId = emisUserSession.EndUserSessionId,
                        
                    });

                if (response.HasSuccessResponse)
                {
                    return new SessionExtendResult.Success();
                }
                _logger.LogError($"{StandardErrorMessage} {response.ErrorForLogging}");
                return new SessionExtendResult.BadGateway();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, $"{StandardErrorMessage} HttpRequestException has been thrown.");
                return new SessionExtendResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}