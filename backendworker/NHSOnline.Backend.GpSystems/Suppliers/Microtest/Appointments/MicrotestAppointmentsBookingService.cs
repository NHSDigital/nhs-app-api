using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments
{
    public class MicrotestAppointmentsBookingService
    {
        private readonly ILogger<MicrotestAppointmentsBookingService> _logger;
        private readonly IMicrotestClient _microtestClient;
        
        public MicrotestAppointmentsBookingService(ILogger<MicrotestAppointmentsBookingService> logger, IMicrotestClient microtestClient)
        {
            _logger = logger;
            _microtestClient = microtestClient;
        }

        public async Task<AppointmentBookResult> Book(MicrotestUserSession userSession, AppointmentBookRequest request)
        {
            try
            {
                _logger.LogEnter();

                if (BookingReasonMissing(request))
                {
                    return new AppointmentBookResult.BadRequest();
                }
                
                var bookAppointmentSlotPostRequest = new BookAppointmentSlotPostRequest(request);

                var response = await _microtestClient.AppointmentsPost(bookAppointmentSlotPostRequest, userSession);
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
        
        private AppointmentBookResult InterpretAppointmentsPostResponse(MicrotestClient.MicrotestApiObjectResponse<string> response)
        {
            if (response.HasSuccessResponse)
            {
                return new AppointmentBookResult.SuccessfullyBooked();
            }

            _logger.LogError(response.ErrorForLogging);
            return new AppointmentBookResult.SupplierSystemUnavailable();
        }

        private bool BookingReasonMissing(AppointmentBookRequest request)
        {
            if (string.IsNullOrEmpty(request.BookingReason))
            {
                _logger.LogError("Booking reason not provided but is mandatory");
                return true;
            }
            return false;
        }
    }
}