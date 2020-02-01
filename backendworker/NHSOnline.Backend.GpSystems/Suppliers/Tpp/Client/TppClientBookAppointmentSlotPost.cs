using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientBookAppointmentSlotPost
        : ITppClientRequest<(TppUserSession userSession, BookAppointment bookAppointment), BookAppointmentReply>
    {
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientBookAppointmentSlotPost(
            TppClientRequestExecutor requestExecutor)
        {
            _requestExecutor = requestExecutor;
        }

        public async Task<TppApiObjectResponse<BookAppointmentReply>> Post(
            (TppUserSession userSession, BookAppointment bookAppointment) parameters)
        {
            var (userSession, bookAppointment) = parameters;
            return await _requestExecutor.Post<BookAppointmentReply>(
                requestBuilder => requestBuilder.Model(bookAppointment).Suid(userSession.Suid));
        }
    }
}