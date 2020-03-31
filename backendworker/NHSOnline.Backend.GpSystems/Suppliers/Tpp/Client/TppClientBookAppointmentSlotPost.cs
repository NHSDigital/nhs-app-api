using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientBookAppointmentSlotPost : ITppClientRequest
        <(TppRequestParameters tppRequestParameters, BookingDates bookingDates, AppointmentBookRequest bookRequest), BookAppointmentReply>
    {
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientBookAppointmentSlotPost(
            TppClientRequestExecutor requestExecutor)
        {
            _requestExecutor = requestExecutor;
        }

        public async Task<TppApiObjectResponse<BookAppointmentReply>>
            Post((TppRequestParameters tppRequestParameters, BookingDates bookingDates, AppointmentBookRequest bookRequest) parameters)
        {
            var (tppRequestParameters, bookingDates, bookRequest) = parameters;
            var bookAppointment = new BookAppointment(tppRequestParameters, bookRequest, bookingDates);

            return await _requestExecutor.Post<BookAppointmentReply>(
                requestBuilder => requestBuilder.Model(bookAppointment).Suid(tppRequestParameters.Suid));
        }
    }
}