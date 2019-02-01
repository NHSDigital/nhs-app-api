using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.SharedModels;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Appointments
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
                    return new AppointmentBookResult.InsufficientPermissions();
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
                return new AppointmentBookResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit();
            }
        }
        
        private AppointmentBookResult InterpretAppointmentsPostResponse(VisionPFSClient.VisionApiObjectResponse<BookAppointmentResponse> response)
        {
            if (response.HasSuccessResponse) return new AppointmentBookResult.SuccessfullyBooked();

            if (response.IsAccessDeniedError)
            {
                return new AppointmentBookResult.InsufficientPermissions();
            }
            
            if (response.IsAppointmentSlotAlreadyBookedError || response.IsAppointmentSlotNotFoundError)
            {
                return new AppointmentBookResult.SlotNotAvailable();
            }

            if (response.IsAppointmentBookingLimitReachedError)
            {
                return new AppointmentBookResult.AppointmentLimitReached();
            }
            
            _logger.LogError($"Call to VISION book appointment endpoint returned an unanticipated error with status code: '{response.StatusCode}'. \n{response.ErrorForLogging}");

            return new AppointmentBookResult.SupplierSystemUnavailable();
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