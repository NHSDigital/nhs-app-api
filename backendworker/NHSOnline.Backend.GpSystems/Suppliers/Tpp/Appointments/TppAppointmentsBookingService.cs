using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Temporal;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments
{
    internal class TppAppointmentsBookingService
    {
        private readonly ILogger<TppAppointmentsBookingService> _logger;
        private readonly ITppClientRequest<(TppUserSession userSession, BookAppointment bookAppointment), BookAppointmentReply> _bookAppointmentSlot;
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;

        public TppAppointmentsBookingService(
            ILogger<TppAppointmentsBookingService> logger,
            ITppClientRequest<(TppUserSession userSession, BookAppointment bookAppointment), BookAppointmentReply> bookAppointmentSlot,
            IDateTimeOffsetProvider dateTimeOffsetProvider)
        {
            _logger = logger;
            _bookAppointmentSlot = bookAppointmentSlot;
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
        }

        public async Task<AppointmentBookResult> Book(GpLinkedAccountModel gpLinkedAccountModel, AppointmentBookRequest request)
        {
            try
            {
                _logger.LogEnter();
                var userSession = (TppUserSession) gpLinkedAccountModel.GpUserSession;

                if (!request.StartTime.HasValue || !request.EndTime.HasValue)
                {
                    _logger.LogError("Appointment book request was missing dates", request);
                    return new AppointmentBookResult.BadRequest();
                }
                
                var bookAppointment = new BookAppointment(userSession, request, _dateTimeOffsetProvider);

                var response = await _bookAppointmentSlot.Post((userSession, bookAppointment));
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

        private AppointmentBookResult InterpretAppointmentsPostResponse(TppApiObjectResponse<BookAppointmentReply> response)
        {
            if (response.HasSuccessResponse)
            {
                return new AppointmentBookResult.Success();
            }

            if (response.ErrorResponse != null)
            {
                switch (response.ErrorResponse.ErrorCode)
                {
                    case TppApiErrorCodes.AppointmentLimitReached:
                        _logger.LogWarning(response.ErrorResponse.UserFriendlyMessage);
                        return new AppointmentBookResult.AppointmentLimitReached();
                    case TppApiErrorCodes.StartDateInPast:
                    case TppApiErrorCodes.SlotNotFound:
                    case TppApiErrorCodes.SlotAlreadyBooked:
                        _logger.LogError(response.ErrorResponse.UserFriendlyMessage);
                        return new AppointmentBookResult.SlotNotAvailable();
                    case TppApiErrorCodes.NoAccess:
                        _logger.LogTppResponseAccessIsForbidden();
                        return new AppointmentBookResult.Forbidden();
                }
            }

            _logger.LogTppUnknownError(response);
            return new AppointmentBookResult.BadGateway();
        }
    }
}
