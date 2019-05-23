using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Session
{
    public class TppSessionExtendService : ISessionExtendService
    {
        private readonly ITppClient _tppClient;
        private readonly ILogger<TppSessionExtendService> _logger;

        private const string StandardErrorMessage = "Failed request retrieving patient selected information for Tpp, while attempting to extend session.";

        public TppSessionExtendService(
            ITppClient tppClient,
            ILogger<TppSessionExtendService> logger)
        {
            _tppClient = tppClient;
            _logger = logger;
        }

        public async Task<SessionExtendResult> Extend(GpUserSession gpUserSession)
        {
            try
            {
                _logger.LogEnter();

                var tppUserSession = (TppUserSession)gpUserSession;
                var patientSelectedResponse = await _tppClient.PatientSelectedPost(tppUserSession);

                if (patientSelectedResponse.HasSuccessResponse)
                {
                    return new SessionExtendResult.Success();
                }

                _logger.LogError($"{StandardErrorMessage} {patientSelectedResponse.ErrorForLogging}");
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
