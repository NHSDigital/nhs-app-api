using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments;
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
                _logger.LogEnter(nameof(Book));
                
                if (!visionUserSession.IsAppointmentsEnabled)
                {
                    _logger.LogError("Appointments not enabled");
                    return new AppointmentBookResult.InsufficientPermissions();
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
                _logger.LogExit(nameof(Book));
            }
        }
        
        private static AppointmentBookResult InterpretAppointmentsPostResponse(VisionClient.VisionApiObjectResponse<BookAppointmentResponse> response)
        {
            if (response.HasSuccessResponse)
            {
                return new AppointmentBookResult.SuccessfullyBooked();
            }
            
// TODO: Error handling NHSO-801

            return new AppointmentBookResult.SupplierSystemUnavailable();
        }
    }
}