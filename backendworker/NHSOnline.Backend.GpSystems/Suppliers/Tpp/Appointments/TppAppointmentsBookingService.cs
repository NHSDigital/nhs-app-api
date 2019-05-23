using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;
using NHSOnline.Backend.Support.Temporal;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments
{
    public class TppAppointmentsBookingService
    {
        private readonly ILogger<TppAppointmentsBookingService> _logger;
        private readonly ITppClient _tppClient;
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;

        public TppAppointmentsBookingService(ILogger<TppAppointmentsBookingService> logger, ITppClient tppClient, IDateTimeOffsetProvider dateTimeOffsetProvider)
        {
            _logger = logger;
            _tppClient = tppClient;
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
        }

        public async Task<AppointmentBookResult> Book(TppUserSession userSession, AppointmentBookRequest request)
        {
            try
            {
                _logger.LogEnter();
            
                if (!request.StartTime.HasValue || !request.EndTime.HasValue)
                {
                    _logger.LogError("Appointment book request was missing dates", request);
                    return new AppointmentBookResult.BadRequest();
                }
                
                var bookAppointment = new BookAppointment(userSession, request, _dateTimeOffsetProvider);

                var response = await _tppClient.BookAppointmentSlotPost(bookAppointment, userSession);
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

        private AppointmentBookResult InterpretAppointmentsPostResponse(TppClient.TppApiObjectResponse<BookAppointmentReply> response)
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
