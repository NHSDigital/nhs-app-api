using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Session
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

        public async Task<SessionExtendResult> Extend(UserSession userSession)
        {
            try
            {
                _logger.LogEnter(nameof(Extend));

                var tppUserSession = (TppUserSession) userSession;
                var patientSelectedResponse = await _tppClient.PatientSelectedPost(tppUserSession);

                if (patientSelectedResponse.HasSuccessResponse)
                {
                    return new SessionExtendResult.SuccessfullyExtended();
                }

                _logger.LogError($"{StandardErrorMessage} {patientSelectedResponse.ErrorForLogging()}");
                return new SessionExtendResult.SupplierSystemUnavailable();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, $"{StandardErrorMessage} HttpRequestException has been thrown.");
                return new SessionExtendResult.SupplierSystemUnavailable();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{StandardErrorMessage} Exception has been thrown.");
                return new SessionExtendResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit(nameof(Extend));
            }
        }
    }
}
