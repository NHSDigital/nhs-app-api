using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientViewAppointmentsPost
        : ITppClientRequest<(TppRequestParameters tppRequestParameters, AppointmentViewType viewType), ViewAppointmentsReply>
    {
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientViewAppointmentsPost(TppClientRequestExecutor requestExecutor)
            => _requestExecutor = requestExecutor;

        public async Task<TppApiObjectResponse<ViewAppointmentsReply>> Post(
            (TppRequestParameters tppRequestParameters, AppointmentViewType viewType) parameters)
        {
            var (tppRequestParameters, viewType) = parameters;
            var viewAppointments = new ViewAppointments(tppRequestParameters, viewType);

            return await _requestExecutor.Post<ViewAppointmentsReply>(
                requestBuilder => requestBuilder.Model(viewAppointments).Suid(tppRequestParameters.Suid));
        }
    }
}