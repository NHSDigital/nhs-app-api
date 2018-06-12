using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public class EmisAppointmentsService : IAppointmentsService
    {
        private readonly IEmisClient _emisClient;
        private readonly IAppointmentsResponseMapper _responseMapper;
        private readonly ILogger _logger;

        public EmisAppointmentsService(IEmisClient emisClient, IAppointmentsResponseMapper responseMapper,
            ILoggerFactory loggerFactory)
        {
            _emisClient = emisClient;
            _responseMapper = responseMapper;
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
                response = await _emisClient.AppointmentsPost(emisHeaders, postRequest);
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError($"Booking appointment slots failed with message {exception.Message}");
                return new AppointmentBookResult.SupplierSystemUnavailable();
            }

            return InterpretAppointmentsPostResponse(response);
        }

        public async Task<AppointmentsResult> GetAppointments(UserSession userSession, bool includePastAppointments,
            DateTimeOffset? pastAppointmentsFromDate)
        {
            var emisUserSession = (EmisUserSession) userSession;

            var emisHeaders = new EmisHeaderParameters
            {
                EndUserSessionId = emisUserSession.EndUserSessionId,
                SessionId = emisUserSession.SessionId
            };

            EmisClient.EmisApiObjectResponse<AppointmentsGetResponse> response;

            try
            {
                response = await _emisClient.AppointmentsGet(emisHeaders, emisUserSession.UserPatientLinkToken,
                    includePastAppointments, pastAppointmentsFromDate);
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError($"Getting my appointments failed with message {exception.Message}");
                return new AppointmentsResult.SupplierSystemUnavailable();
            }

            return InterpretAppointmentsGetResponse(response);
        }

        private AppointmentsResult InterpretAppointmentsGetResponse(
            EmisClient.EmisApiObjectResponse<AppointmentsGetResponse> response)
        {
            if (response.HasSuccessStatusCode)
            {
                try
                {
                    return new AppointmentsResult.SuccessfullyRetrieved(_responseMapper.Map(response.Body));
                }
                catch (Exception e)
                {
                    _logger.LogError(
                        $"Something went wrong during building the response. Exception message: {e.Message}");
                    return new AppointmentsResult.InternalServerError();
                }
            }

            if (HasInsufficientPermissions(response))
            {
                return new AppointmentsResult.SuccessfullyRetrieved(new AppointmentsResponse());
            }

            return new AppointmentsResult.SupplierSystemUnavailable();
        }

        private static AppointmentBookResult InterpretAppointmentsPostResponse(
            EmisClient.EmisApiResponse response)
        {
            if (response.HasSuccessStatusCode)
            {
                return new AppointmentBookResult.SuccessfullyBooked();
            }

            if (IsNotAvailableForBooking(response) || IsInThePast(response) || NotFound(response))
            {
                return new AppointmentBookResult.SlotNotAvailable();
            }

            if (response.HasForbiddenResponse())
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