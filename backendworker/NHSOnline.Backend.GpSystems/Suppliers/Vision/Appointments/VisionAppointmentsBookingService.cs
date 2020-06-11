using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.SharedModels;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Appointments
{
    public class VisionAppointmentsBookingService
    {
        private readonly ILogger<VisionAppointmentsBookingService> _logger;
        private readonly IVisionClient _visionClient;
        
        public VisionAppointmentsBookingService(ILogger<VisionAppointmentsBookingService> logger, IVisionClient visionClient)
        {
            _logger = logger;
            _visionClient = visionClient;
        }
        
        public async Task<AppointmentBookResult> Book(VisionUserSession visionUserSession, AppointmentBookRequest request)
        {
            try
            {
                _logger.LogEnter();
                
                if (!visionUserSession.IsAppointmentsEnabled)
                {
                    _logger.LogError("Appointments not enabled");
                    return new AppointmentBookResult.Forbidden();
                }

                if (ReasonWillNotBeAccepted(visionUserSession, request))
                {
                    return new AppointmentBookResult.BadRequest();
                }

                var bookAppointmentRequest = new BookAppointmentRequest(visionUserSession, request);
                
                var response = await _visionClient.BookAppointment(
                    visionUserSession,
                    bookAppointmentRequest
                );
                return InterpretAppointmentsPostResponse(response);
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "Booking appointment slots failed.");
                return new AppointmentBookResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }
        }
        
        private AppointmentBookResult InterpretAppointmentsPostResponse(VisionPfsApiObjectResponse<BookAppointmentResponse> response)
        {
            if (response.HasSuccessResponse)
            {
                return new AppointmentBookResult.Success();
            }

            if (response.IsAccessDeniedError)
            {
                return new AppointmentBookResult.Forbidden();
            }
            
            if (response.IsAppointmentSlotAlreadyBookedError || response.IsAppointmentSlotNotFoundError)
            {
                return new AppointmentBookResult.SlotNotAvailable();
            }

            if (response.IsAppointmentBookingLimitReachedError)
            {
                return new AppointmentBookResult.AppointmentLimitReached();
            }

            if (response.UnparsableResultMessage != null)
            {
                return new AppointmentBookResult.InternalServerError();
            }
            
            _logger.LogError($"Call to VISION book appointment endpoint returned an unanticipated error with status code: '{response.StatusCode}'. \n{response.ErrorForLogging}");
            _logger.LogVisionErrorResponse(response);

            return new AppointmentBookResult.BadGateway();
        }

        private bool ReasonWillNotBeAccepted(VisionUserSession visionUserSession, AppointmentBookRequest request)
        {
            if (!string.IsNullOrEmpty(request.BookingReason) &&
                visionUserSession.AppointmentBookingReasonNecessity == Necessity.NotAllowed)
            {
                _logger.LogError($"Booking reason '{request.BookingReason}' provided but is not allowed");
                return true;
            }
            return false;
        }
    }
}
