using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Session
{
    internal class TppSessionExtendService : ISessionExtendService
    {
        private readonly ITppClientRequest<TppRequestParameters, PatientSelectedReply> _patientSelected;
        private readonly ILogger<TppSessionExtendService> _logger;

        private const string StandardErrorMessage = "Failed request retrieving patient selected information for Tpp, while attempting to extend session.";

        public TppSessionExtendService(
            ITppClientRequest<TppRequestParameters, PatientSelectedReply> patientSelected,
            ILogger<TppSessionExtendService> logger)
        {
            _patientSelected = patientSelected;
            _logger = logger;
        }

        public async Task<SessionExtendResult> Extend(GpLinkedAccountModel gpLinkedAccountModel)
        {
            try
            {
                _logger.LogEnter();

                var tppRequestParameters = gpLinkedAccountModel.BuildTppRequestParameters(_logger);

                var patientSelectedResponse = await _patientSelected.Post(tppRequestParameters);

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