using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientCancelAppointmentPost : ITppClientRequest
            <(TppRequestParameters tppRequestParameters, AppointmentCancelRequest cancelRequest), CancelAppointmentReply>
    {
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientCancelAppointmentPost(TppClientRequestExecutor requestExecutor)
            => _requestExecutor = requestExecutor;

        public async Task<TppApiObjectResponse<CancelAppointmentReply>> Post(
            (TppRequestParameters tppRequestParameters, AppointmentCancelRequest cancelRequest) parameters)
        {
            var (tppRequestParameters, cancelRequest) = parameters;
            var cancelAppointment = new CancelAppointment(tppRequestParameters, cancelRequest);

            return await _requestExecutor.Post<CancelAppointmentReply>(
                requestBuilder => requestBuilder.Model(cancelAppointment).Suid(tppRequestParameters.Suid));
        }
    }
}