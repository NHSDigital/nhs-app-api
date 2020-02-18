using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments
{
    internal class TppAppointmentsCancellationService
    {
        private readonly ILogger<TppAppointmentsCancellationService> _logger;
        private readonly ITppClientRequest<(CancelAppointment cancelAppointment, string suid), CancelAppointmentReply> _cancelAppointment;

        public TppAppointmentsCancellationService(
            ILogger<TppAppointmentsCancellationService> logger,
            ITppClientRequest<(CancelAppointment cancelAppointment, string suid), CancelAppointmentReply> cancelAppointment)
        {
            _logger = logger;
            _cancelAppointment = cancelAppointment;
        }

        public async Task<AppointmentCancelResult> Cancel(GpLinkedAccountModel gpLinkedAccountModel, AppointmentCancelRequest request)
        {
            try
            {
                _logger.LogEnter();
                var userSession = (TppUserSession) gpLinkedAccountModel.GpUserSession;

                var postRequest = new CancelAppointment(userSession, request);
                
                var response = await _cancelAppointment.Post((postRequest, userSession.Suid));
                return InterpretCancelAppointmentReply(response);
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "Cancelling appointment failed.");
                return new AppointmentCancelResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private AppointmentCancelResult InterpretCancelAppointmentReply(TppApiObjectResponse<CancelAppointmentReply> response)
        {
            if (response.HasSuccessResponse)
            {
                return new AppointmentCancelResult.Success();
            }

            if (response.ErrorResponse != null)
            {
                switch (response.ErrorResponse.ErrorCode)
                {
                    case TppApiErrorCodes.StartDateInPast:
                        _logger.LogError(response.ErrorResponse.UserFriendlyMessage);
                        return new AppointmentCancelResult.AppointmentNotCancellable();
                    case TppApiErrorCodes.AppointmentWithinOneHour:
                        _logger.LogError(response.ErrorResponse.UserFriendlyMessage);
                        return new AppointmentCancelResult.TooLateToCancel();
                    case TppApiErrorCodes.NoAccess:
                        _logger.LogTppResponseAccessIsForbidden();
                        return new AppointmentCancelResult.Forbidden();
                }
            }
            
            _logger.LogTppUnknownError(response);
            return new AppointmentCancelResult.BadGateway();
        }
    }
}
