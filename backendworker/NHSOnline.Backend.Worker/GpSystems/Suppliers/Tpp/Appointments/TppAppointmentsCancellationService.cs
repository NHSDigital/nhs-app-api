using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Support.Logging;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments
{
    public class TppAppointmentsCancellationService
    {
        private readonly ILogger<TppAppointmentsCancellationService> _logger;
        private readonly ITppClient _tppClient;

        public TppAppointmentsCancellationService(ILogger<TppAppointmentsCancellationService> logger, ITppClient tppClient)
        {
            _logger = logger;
            _tppClient = tppClient;
        }

        public async Task<AppointmentCancelResult> Cancel(TppUserSession userSession, AppointmentCancelRequest request)
        {
            try
            {
                _logger.LogEnter(nameof(Cancel));
            
                var postRequest = new CancelAppointment(userSession, request);
                
                var response = await _tppClient.CancelAppointmentPost(postRequest, userSession.Suid);
                return InterpretCancelAppointmentReply(response);
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "Cancelling appointment failed.");
                return new AppointmentCancelResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit(nameof(Cancel));
            }
        }

        private AppointmentCancelResult InterpretCancelAppointmentReply(TppClient.TppApiObjectResponse<CancelAppointmentReply> response)
        {
            if (response.HasSuccessResponse)
            {
                return new AppointmentCancelResult.SuccessfullyCancelled();
            }

            if (response.ErrorResponse != null)
            {
                switch (response.ErrorResponse.ErrorCode)
                {
                    case TppApiErrorCodes.StartDateInPast:
                    case TppApiErrorCodes.AppointmentWithinOneHour:
                        _logger.LogError(response.ErrorResponse.UserFriendlyMessage);
                        return new AppointmentCancelResult.AppointmentNotCancellable();
                    case TppApiErrorCodes.NoAccess:
                        _logger.LogTppResponseAccessIsForbidden();
                        return new AppointmentCancelResult.InsufficientPermissions();
                    default:
                        break;
                }
            }
            
            _logger.LogTppUnknownError(response);
            return new AppointmentCancelResult.SupplierSystemUnavailable();
        }
    }
}
