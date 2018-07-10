using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments;
using NHSOnline.Backend.Worker.Support.Date;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments
{
    public class TppAppointmentsServiceBook
    {
        private readonly ILogger _logger;
        private readonly ITppClient _tppClient;
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;

        public TppAppointmentsServiceBook(ILogger logger, ITppClient tppClient, IDateTimeOffsetProvider dateTimeOffsetProvider)
        {
            _logger = logger;
            _tppClient = tppClient;
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
        }

        public async Task<AppointmentBookResult> Book(TppUserSession userSession, AppointmentBookRequest request)
        {
            if (!request.StartTime.HasValue || !request.EndTime.HasValue)
            {
                _logger.LogError("Appointment book request was missing dates", request);
                return new AppointmentBookResult.BadRequest();
            }
            try
            {
                var bookAppointment = new BookAppointment(userSession, request, _dateTimeOffsetProvider);

                var response = await _tppClient.BookAppointmentSlotPost(bookAppointment, userSession);
                return InterpretAppointmentsPostResponse(response);
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "Booking appointment slots failed.");
                return new AppointmentBookResult.SupplierSystemUnavailable();
            }
        }

        private AppointmentBookResult InterpretAppointmentsPostResponse(TppClient.TppApiObjectResponse<BookAppointmentReply> response)
        {
            if (response.HasSuccessResponse)
            {
                return new AppointmentBookResult.SuccessfullyBooked();
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
                        return new AppointmentBookResult.InsufficientPermissions();
                    default:
                        break;
                }
            }

            _logger.LogTppUnknownError(response);
            return new AppointmentBookResult.SupplierSystemUnavailable();
        }
    }
}
