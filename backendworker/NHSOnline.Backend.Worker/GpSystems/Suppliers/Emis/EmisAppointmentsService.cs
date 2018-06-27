using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Resources;
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

        public async Task<AppointmentCancelResult> Cancel(UserSession userSession, AppointmentCancelRequest request)
        {
            var emisUserSession = (EmisUserSession) userSession;

            if (!TryGetCancellationReason(request.CancellationReasonId, out var cancellationReasonText))
            {
                _logger.LogError("Supplied cancellation reason ID '{0}' was not found in cancellation reasons resource file.", request.CancellationReasonId);
                return new AppointmentCancelResult.BadRequest();
            }

            if (!long.TryParse(request.AppointmentId, out var slotId))
            {
                _logger.LogError("Supplied appointment ID '{0}' could not be converted to a 64-bit integer.", request.AppointmentId);
                return new AppointmentCancelResult.BadRequest();
            }

            var deleteRequest = new CancelAppointmentDeleteRequest
            {
                UserPatientLinkToken = emisUserSession.UserPatientLinkToken,
                CancellationReason = cancellationReasonText,
                SlotId = slotId
            };

            var emisHeaders = new EmisHeaderParameters
            {
                EndUserSessionId = emisUserSession.EndUserSessionId,
                SessionId = emisUserSession.SessionId,
            };

            EmisClient.EmisApiObjectResponse<CancelAppointmentDeleteResponse> response;

            try
            {
                response = await _emisClient.AppointmentsDelete(emisHeaders, deleteRequest);
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError($"Cancelling appointment failed with message {exception.Message}");
                return new AppointmentCancelResult.SupplierSystemUnavailable();
            }

            return InterpretAppointmentsDeleteResponse(response);
        }

        private static bool TryGetCancellationReason(string requestCancellationReasonId, out string cancellationReasonText)
        {
            cancellationReasonText = string.Empty;

            if (string.IsNullOrWhiteSpace(requestCancellationReasonId))
            {
                return false;
            }

            var resourceManager = new ResourceManager(typeof(CancellationReasons));
            var resourceSet = resourceManager.GetResourceSet(CultureInfo.CurrentCulture, true, true);
            cancellationReasonText = resourceSet.GetString(requestCancellationReasonId);

            return cancellationReasonText != null;
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

            if (response.HasForbiddenResponse())
            {
                return new AppointmentsResult.CannotViewAppointments();
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

            if (SlotIsNotAvailableForBooking(response) || SlotIsInThePast(response) || SlotNotFound(response))
            {
                return new AppointmentBookResult.SlotNotAvailable();
            }

            if (response.HasForbiddenResponse())
            {
                return new AppointmentBookResult.InsufficientPermissions();
            }

            return new AppointmentBookResult.SupplierSystemUnavailable();
        }

        private static AppointmentCancelResult InterpretAppointmentsDeleteResponse(
            EmisClient.EmisApiResponse response)
        {
            if (response.HasSuccessStatusCode)
            {
                return new AppointmentCancelResult.SuccessfullyCancelled();
            }

            if (AppointmentIsNotAvailableForCancelling(response) || AppointmentIsInThePast(response) || AppointmentNotFound(response))
            {
                return new AppointmentCancelResult.AppointmentNotCancellable();
            }

            if (response.HasForbiddenResponse())
            {
                return new AppointmentCancelResult.InsufficientPermissions();
            }

            return new AppointmentCancelResult.SuccessfullyCancelled();
        }

        private static bool SlotIsNotAvailableForBooking(EmisClient.EmisApiResponse response)
        {
            return response.StatusCode == HttpStatusCode.Conflict;
        }

        private static bool SlotNotFound(EmisClient.EmisApiResponse response)
        {
            return response.HasExceptionWithMessage(
                EmisApiErrorMessages.AppointmentsPost_NotFound);
        }

        private static bool SlotIsInThePast(EmisClient.EmisApiResponse response)
        {
            return response.HasExceptionWithMessage(
                EmisApiErrorMessages.AppointmentsPost_InThePast);
        }

        private static bool AppointmentIsNotAvailableForCancelling(EmisClient.EmisApiResponse response)
        {
            return response.StatusCode == HttpStatusCode.Conflict;
        }

        private static bool AppointmentIsInThePast(EmisClient.EmisApiResponse response)
        {
            return response.HasExceptionWithMessage(
                EmisApiErrorMessages.AppointmentsDelete_InThePast);
        }

        private static bool AppointmentNotFound(EmisClient.EmisApiResponse response)
        {
            return response.HasExceptionWithMessage(
                EmisApiErrorMessages.AppointmentsDelete_NotFound);
        }
    }
}