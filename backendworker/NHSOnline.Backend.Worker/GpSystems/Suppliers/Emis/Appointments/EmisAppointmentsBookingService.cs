using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Support.Logging;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.Areas.SharedModels;
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
                _logger.LogEnter();

                if (BookingReasonInvalid(emisUserSession, request))
                {
                    return new AppointmentBookResult.BadRequest();
                }

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
                _logger.LogExit();
            }
        }

        private AppointmentBookResult InterpretAppointmentsPostResponse(
            EmisClient.EmisApiResponse response)
        {   
            if (response.HasSuccessResponse)
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
            
            if (TelephoneNumberIsBlank(response))
            {
                return new AppointmentBookResult.BadRequest();
            }

            if (response.HasForbiddenResponse())
            {
                _logger.LogEmisResponseIsForbidden();
                _logger.LogEmisErrorResponse(response);
                return new AppointmentBookResult.InsufficientPermissions();
            }
            
            _logger.LogEmisUnknownError(response);
            _logger.LogEmisErrorResponse(response);
            return new AppointmentBookResult.SupplierSystemUnavailable();
        }

        private bool TelephoneNumberIsBlank(EmisClient.EmisApiResponse response)
        {
            var check = response.HasStatusCodeAndErrorCode(HttpStatusCode.BadRequest,
                            EmisApiErrorCode.RequiredFieldValueMissing)
                        || response.HasExceptionWithMessage(EmisApiErrorMessages.EmisService_TelephoneNumberRequired);
            if (check)
            {
                _logger.LogError("Telephone number is blank.");
                _logger.LogEmisErrorResponse(response);
            }
            return check;
        }

        private bool SlotIsNotAvailableForBooking(EmisClient.EmisApiResponse response)
        {
            var check = response.StatusCode == HttpStatusCode.Conflict;
            if (check)
            {
                _logger.LogError("Slot is not available for booking.");
                _logger.LogEmisErrorResponse(response);
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
                _logger.LogEmisErrorResponse(response);
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
                _logger.LogEmisErrorResponse(response);
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
                _logger.LogEmisErrorResponse(response);
            }
            return check;
        }

        private bool BookingReasonInvalid(EmisUserSession emisUserSession, AppointmentBookRequest request)
            => ReasonWillNotBeAccepted(emisUserSession, request) || ReasonRequiredButNotProvided(emisUserSession, request);

        private bool ReasonWillNotBeAccepted(EmisUserSession emisUserSession, AppointmentBookRequest request)
        {
            if (!string.IsNullOrEmpty(request.BookingReason) &&
                emisUserSession.AppointmentBookingReasonNecessity == Necessity.NotAllowed)
            {
                _logger.LogError($"Booking reason '{request.BookingReason}' provided but is not allowed");
                return true;
            }
            return false;
        }

        private bool ReasonRequiredButNotProvided(EmisUserSession emisUserSession, AppointmentBookRequest request)
        {
            if (string.IsNullOrEmpty(request.BookingReason) &&
                emisUserSession.AppointmentBookingReasonNecessity == Necessity.Mandatory)
            {
                _logger.LogError($"Booking reason not provided but is mandatory");
                return true;
            }
            return false;
        }
    }
}
