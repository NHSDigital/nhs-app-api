using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientListSlotsPost
        : ITppClientRequest<(TppRequestParameters tppRequestParameters, AppointmentSlotsDateRange dateRange), ListSlotsReply>
    {
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientListSlotsPost(TppClientRequestExecutor requestExecutor)
            => _requestExecutor = requestExecutor;

        public async Task<TppApiObjectResponse<ListSlotsReply>> Post((TppRequestParameters tppRequestParameters, AppointmentSlotsDateRange dateRange) parameters)
        {
            var (tppRequestParameters, dateRange) = parameters;
            var listSlotsRequest = new ListSlots(tppRequestParameters, dateRange);

            return await _requestExecutor.Post<ListSlotsReply>(
                requestBuilder => requestBuilder.Model(listSlotsRequest).Suid(tppRequestParameters.Suid));
        }
    }
}