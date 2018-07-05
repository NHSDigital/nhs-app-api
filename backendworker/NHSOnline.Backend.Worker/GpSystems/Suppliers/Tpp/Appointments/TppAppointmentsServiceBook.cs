using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments
{
    public class TppAppointmentsServiceBook
    {
        private readonly ILogger _logger;
        private readonly ITppClient _tppClient;

        public TppAppointmentsServiceBook(ILogger logger, ITppClient tppClient)
        {
            _logger = logger;
            _tppClient = tppClient;
        }

        public async Task<AppointmentBookResult> Book(TppUserSession userSession, AppointmentBookRequest request)
        {
            if (!request.StartTime.HasValue || !request.EndTime.HasValue)
            {
                return new AppointmentBookResult.BadRequest();
            }
            try
            {
                var bookAppointment = new BookAppointment(userSession, request)
                {
                    
                };

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
                    case TppApiErrorCodes.StartDateInPast:
                    case TppApiErrorCodes.SlotNotFound:
                    case TppApiErrorCodes.SlotAlreadyBooked:
                        _logger.LogError(response.ErrorResponse.UserFriendlyMessage);
                        return new AppointmentBookResult.SlotNotAvailable();
                    case TppApiErrorCodes.NoAccess:
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
