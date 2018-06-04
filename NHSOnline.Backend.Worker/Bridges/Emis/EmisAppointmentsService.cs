using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.Bridges.Emis.Models;
using NHSOnline.Backend.Worker.Router.Appointments;

namespace NHSOnline.Backend.Worker.Bridges.Emis
{
    public class EmisAppointmentsService: IAppointmentsService
    {
        private readonly IEmisClient _emisClient;
        private readonly ILogger _logger;
        
        public EmisAppointmentsService(IEmisClient emisClient, ILoggerFactory loggerFactory)
        {
            _emisClient = emisClient;
            _logger = loggerFactory.CreateLogger<EmisAppointmentsService>();
        }
        
        public async Task<AppointmentBookResult> Book(UserSession userSession, AppointmentBookRequest request)
        {
            var emisUserSession = (EmisUserSession) userSession;
            var postRequest = new BookAppointmentSlotPostRequest
            {
                UserPatientLinkToken = emisUserSession.UserPatientLinkToken,
                BookingReason = request.BookingReason,
                SlotId = Convert.ToInt64(request.SlotId)
            };

            var emisHeaders = new EmisHeaderParameters
            {
                EndUserSessionId = emisUserSession.EndUserSessionId,
                SessionId = emisUserSession.SessionId,
            };

            EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse> response;
            
            try
            {
                response = await _emisClient.AppointmentPost(emisHeaders, postRequest);  
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError($"Booking appointment slots failed with message {exception.Message}");
                return new AppointmentBookResult.SupplierSystemUnavailable();
            }

            if (response.HasSuccessStatusCode)
            {
                return new AppointmentBookResult.SuccessfullyBooked();
            }

            return GetCorrectErrorResult(response);
        }

        private static AppointmentBookResult GetCorrectErrorResult(
            EmisClient.EmisApiResponse response)
        {
            if (IsNotAvailableForBooking(response) || IsInThePast(response) || NotFound(response))
            {
                return new AppointmentBookResult.SlotNotAvailable();
            }

            if (HasInsufficientPermissions(response))
            {
                return new AppointmentBookResult.InsufficientPermissions();
            }
            
            return new AppointmentBookResult.SupplierSystemUnavailable();
        }

        private static bool IsNotAvailableForBooking(EmisClient.EmisApiResponse response)
        {
            return response.StatusCode == HttpStatusCode.Conflict;
        }
        
        private static bool NotFound(EmisClient.EmisApiResponse response)
        {
            return response.HasExceptionWithMessage(
                EmisApiErrorMessages.AppointmentsPost_NotFound);
        }
        
        private static bool IsInThePast(EmisClient.EmisApiResponse response)
        {
            return response.HasExceptionWithMessage(
                EmisApiErrorMessages.AppointmentsPost_InThePast);
        }

        private static bool HasInsufficientPermissions(EmisClient.EmisApiResponse response)
        {
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return true;
            }

            return response.HasExceptionWithMessageContaining(
                EmisApiErrorMessages.EmisService_NotEnabledForUser);
        }
    }
}
