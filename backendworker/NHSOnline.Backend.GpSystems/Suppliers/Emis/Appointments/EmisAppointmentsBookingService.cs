using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.SharedModels;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Appointments
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

                var postRequest = new BookAppointmentSlotPostRequest(emisUserSession.UserPatientLinkToken, request);
                var emisHeaders = new EmisHeaderParameters(emisUserSession);

                var response = await _emisClient.AppointmentsPost(emisHeaders, postRequest);
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

        private AppointmentBookResult InterpretAppointmentsPostResponse(
            EmisClient.EmisApiResponse response)
        {   
            if (response.HasSuccessResponse)
            {
                return new AppointmentBookResult.Success();
            }

            if (SlotIsNotAvailableForBooking(response) || 
                SlotIsInThePast(response) ||
                SlotNotFound(response) ||
                SlotIsOutsidePracticeDefinedDays(response))
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
                return new AppointmentBookResult.Forbidden();
            }
            
            _logger.LogEmisUnknownError(response);
            _logger.LogEmisErrorResponse(response);
            return new AppointmentBookResult.BadGateway();
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
                _logger.LogWarning("Slot is not available for booking.");
                _logger.LogEmisLogWarningResponse(response);
            }
            return check;
        }

        private bool SlotIsOutsidePracticeDefinedDays(EmisClient.EmisApiResponse response)
        {
            var check = response.HasStatusCodeAndErrorCode(HttpStatusCode.BadRequest,
                            EmisApiErrorCode.AppointmentSlotIsAfterPracticeDefinedDays) ||
                        response.HasStatusCodeAndErrorCode(HttpStatusCode.BadRequest,
                            EmisApiErrorCode.AppointmentSlotIsBeforePracticeDefinedDays);
            if (check)
            {
                _logger.LogWarning("Slot is outside practice defined date range.");
                _logger.LogEmisLogWarningResponse(response);
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
                _logger.LogWarning("Slot is in the past.");
                _logger.LogEmisLogWarningResponse(response);
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
                _logger.LogWarning("Booking limit reached.");
                _logger.LogEmisWarningResponse(response);
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
