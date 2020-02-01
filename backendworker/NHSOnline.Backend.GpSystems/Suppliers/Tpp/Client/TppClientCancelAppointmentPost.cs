using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientCancelAppointmentPost
        : ITppClientRequest<(CancelAppointment cancelAppointment, string suid), CancelAppointmentReply>
    {
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientCancelAppointmentPost(TppClientRequestExecutor requestExecutor)
            => _requestExecutor = requestExecutor;

        public async Task<TppApiObjectResponse<CancelAppointmentReply>> Post(
            (CancelAppointment cancelAppointment, string suid) parameters)
        {
            var (cancelAppointment, suid) = parameters;

            return await _requestExecutor.Post<CancelAppointmentReply>(
                requestBuilder => requestBuilder.Model(cancelAppointment).Suid(suid));
        }
    }
}