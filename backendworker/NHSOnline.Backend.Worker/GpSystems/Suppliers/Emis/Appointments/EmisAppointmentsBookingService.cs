using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Support.Logging;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments
{
    public class EmisAppointmentsBookingService
    {
        private readonly ILogger<EmisAppointmentsBookingService> _logger;
        private readonly IEmisClient _emisClient;

        public EmisAppointmentsBookingService(ILogger<EmisAppointmentsBookingService> logger, IEmisClient emisClient)
        {
            _logger = logger;
            _emisClient = emisClient;
        }

        public async Task<AppointmentBookResult> Book(EmisUserSession emisUserSession, AppointmentBookRequest request)
        {
            try
            {
                _logger.LogEnter(nameof(Book));
            
                var postRequest = new BookAppointmentSlotPostRequest(emisUserSession, request);
                var emisHeaders = new EmisHeaderParameters(emisUserSession);
                
                var response = await _emisClient.AppointmentsPost(emisHeaders, postRequest);
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
        
        private AppointmentBookResult InterpretAppointmentsPostResponse(
            EmisClient.EmisApiResponse response)
        {
            if (response.HasSuccessStatusCode)
            {
                return new AppointmentBookResult.SuccessfullyBooked();
            }

            if (SlotIsNotAvailableForBooking(response) || 
                SlotIsInThePast(response) ||
                SlotNotFound(response))
            {
                return new AppointmentBookResult.SlotNotAvailable();
            }

            if (BookingLimitReached(response))
            {
                return new AppointmentBookResult.AppointmentLimitReached();
            }

            if (response.HasForbiddenResponse())
            {
                _logger.LogEmisResponseIsForbidden();
                return new AppointmentBookResult.InsufficientPermissions();
            }
            
            _logger.LogEmisUnknownError(response);
            return new AppointmentBookResult.SupplierSystemUnavailable();
        }

        private bool SlotIsNotAvailableForBooking(EmisClient.EmisApiResponse response)
        {
            var check = response.StatusCode == HttpStatusCode.Conflict;
            if (check)
            {
                _logger.LogError("Slot is not available for booking.");
            }
            return check;
        }

        private bool SlotNotFound(EmisClient.EmisApiResponse response)
        {
            var check = (response.StatusCode == HttpStatusCode.NotFound)
                        || response.HasExceptionWithMessage(EmisApiErrorMessages.AppointmentsPost_NotFound);
            if (check)
            {
                _logger.LogError("Slot not found.");
            }
            return check;
        }

        private bool SlotIsInThePast(EmisClient.EmisApiResponse response)
        {
            var check = response.HasStatusCodeAndErrorCode(HttpStatusCode.BadRequest,
                            EmisApiErrorCode.ProvidedAppointmentSlotInPast)
                        || response.HasExceptionWithMessage(EmisApiErrorMessages.AppointmentsPost_InThePast);
            if (check)
            {
                _logger.LogError("Slot is in the past.");
            }
            return check;
        }

        private bool BookingLimitReached(EmisClient.EmisApiResponse response)
        {
            var check = response.HasStatusCodeAndErrorCode(HttpStatusCode.BadRequest,
                            EmisApiErrorCode.OnlineUserMaxAppointmentBookCount)
                        || response.HasExceptionWithMessageContaining(
                            EmisApiErrorMessages.EmisService_BookedAppointmentLimit);

            if (check)
            {
                _logger.LogError("Slot is in the past.");
            }
            return check;
        }
    }
}
