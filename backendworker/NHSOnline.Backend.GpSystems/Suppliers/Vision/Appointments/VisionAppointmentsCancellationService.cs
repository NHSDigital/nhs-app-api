using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Appointments
{
    public class VisionAppointmentsCancellationService
    {
        private readonly ILogger<VisionAppointmentsCancellationService> _logger;
        private readonly IVisionClient _visionClient;
        
        public VisionAppointmentsCancellationService(ILogger<VisionAppointmentsCancellationService> logger, IVisionClient client)
        {
            _logger = logger;
            _visionClient = client;
        }
        
        public async Task<AppointmentCancelResult> Cancel(VisionUserSession visionUserSession, AppointmentCancelRequest request)
        {
            try
            {
                _logger.LogEnter();

                if (!visionUserSession.IsAppointmentsEnabled)
                {
                    _logger.LogError("Appointment Cancellation not enabled");
                    return new AppointmentCancelResult.InsufficientPermissions();
                }
            
                var cancelAppointmentRequest = new CancelAppointmentRequest
                {
                    PatientId = visionUserSession.PatientId,
                    SlotId = request.AppointmentId,
                    ReasonId = request.CancellationReasonId
                };
                
                var response = await _visionClient.CancelAppointment(
                    visionUserSession,
                    cancelAppointmentRequest
                );
                
                return InterpretCancelAppointmentResponse(response);
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "Cancelling appointment failed.");
                return new AppointmentCancelResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private AppointmentCancelResult InterpretCancelAppointmentResponse(VisionPFSClient.VisionApiObjectResponse<CancelledAppointmentResponse> response)
        {
            if (response.HasSuccessResponse) return new AppointmentCancelResult.SuccessfullyCancelled();

            if (response.IsAccessDeniedError)
            {
                return new AppointmentCancelResult.InsufficientPermissions();
            }

            if (response.IsAppointmentSlotNotBookedToCurrentUserError || response.IsAppointmentSlotNotFoundError)
            {
                return new AppointmentCancelResult.AppointmentNotCancellable();
            }

            if (response.UnparsableResultMessage != null)
            {
                return new AppointmentCancelResult.InternalServerError();
            }
            
            _logger.LogError($"Call to VISION cancel appointment endpoint returned an unanticipated error with status code: '{response.StatusCode}'. \n{response.ErrorForLogging}");
            _logger.LogVisionErrorResponse(response);

            return new AppointmentCancelResult.SupplierSystemUnavailable();
        }
    }
}
