using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientViewAppointmentsPost
        : ITppClientRequest<(ViewAppointments viewAppointments, string suid), ViewAppointmentsReply>
    {
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientViewAppointmentsPost(TppClientRequestExecutor requestExecutor)
            => _requestExecutor = requestExecutor;

        public async Task<TppApiObjectResponse<ViewAppointmentsReply>> Post(
            (ViewAppointments viewAppointments, string suid) parameters)
        {
            var (viewAppointments, suid) = parameters;

            return await _requestExecutor.Post<ViewAppointmentsReply>(
                requestBuilder => requestBuilder.Model(viewAppointments).Suid(suid));
        }
    }
}