using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Appointments
{
    public class EmisAppointmentsCancellationService
    {
        private readonly IEmisClient _emisClient;
        private readonly ILogger<EmisAppointmentsCancellationService> _logger;
        private readonly ICancellationReasonService _cancellationReasonService;

        public EmisAppointmentsCancellationService(
            ILogger<EmisAppointmentsCancellationService> logger,
            IEmisClient emisClient,
            ICancellationReasonService cancellationReasonService)
        {
            _emisClient = emisClient;
            _logger = logger;
            _cancellationReasonService = cancellationReasonService;
        }

        public async Task<AppointmentCancelResult> Cancel(
            GpLinkedAccountModel gpLinkedAccountModel,
            AppointmentCancelRequest request)
        {
            try
            {
                _logger.LogEnter();
                var emisRequestParameters = gpLinkedAccountModel.BuildEmisRequestParameters(_logger);

                if (_cancellationReasonService.TryGetCancellationReason(request.CancellationReasonId, out var cancellationReason) &&
                    TryGetAppointmentId(request, out var slotId))
                {
                    var response = await _emisClient.AppointmentsDelete(emisRequestParameters, slotId, cancellationReason);
                    return InterpretAppointmentsDeleteResponse(response);
                }

                return new AppointmentCancelResult.BadRequest();
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "Cancelling appointment failed");
                return new AppointmentCancelResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private bool TryGetAppointmentId(AppointmentCancelRequest request, out long slotId)
        {
            if (long.TryParse(request.AppointmentId, out slotId))
            {
                return true;
            }
            _logger.LogError(
                $"Supplied appointment ID '{request.AppointmentId}' could not be converted to a 64-bit integer.");
            return false;
        }

        private AppointmentCancelResult InterpretAppointmentsDeleteResponse(
            EmisClient.EmisApiResponse response)
        {
            if (response.HasSuccessResponse)
            {
                return new AppointmentCancelResult.Success();
            }

            if (AppointmentIsNotAvailableForCancelling(response) ||
                AppointmentIsInThePast(response) ||
                AppointmentNotFound(response))
            {
                return new AppointmentCancelResult.AppointmentNotCancellable();
            }

            if (response.HasForbiddenResponse())
            {
                _logger.LogEmisResponseIsForbidden();
                _logger.LogEmisErrorResponse(response);
                return new AppointmentCancelResult.Forbidden();
            }

            _logger.LogEmisUnknownError(response);
            _logger.LogEmisErrorResponse(response);
            return new AppointmentCancelResult.BadGateway();
        }


        private bool AppointmentIsNotAvailableForCancelling(EmisClient.EmisApiResponse response)
        {
            var check = response.StatusCode == HttpStatusCode.Conflict;
            if (check)
            {
                _logger.LogError("Slot is not available for cancelling.");
                _logger.LogEmisErrorResponse(response);
            }
            return check;
        }

        private bool AppointmentIsInThePast(EmisClient.EmisApiResponse response)
        {
            var check = response.StatusCode == HttpStatusCode.BadRequest
                        || response.HasExceptionWithMessage(EmisApiErrorMessages.AppointmentsDelete_InThePast);
            if (check)
            {
                _logger.LogError("Appointment is in the past.");
                _logger.LogEmisErrorResponse(response);
            }
            return check;
        }

        private bool AppointmentNotFound(EmisClient.EmisApiResponse response)
        {
            var check = response.StatusCode == HttpStatusCode.NotFound
                        || response.HasExceptionWithMessage(EmisApiErrorMessages.AppointmentsDelete_NotFound);
            if (check)
            {
                _logger.LogError("Appointment not found.");
                _logger.LogEmisErrorResponse(response);
            }

            return check;
        }
    }
}
