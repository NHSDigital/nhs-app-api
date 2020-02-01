using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientListSlotsPost
        : ITppClientRequest<(ListSlots listSlots, string suid), ListSlotsReply>
    {
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientListSlotsPost(TppClientRequestExecutor requestExecutor)
            => _requestExecutor = requestExecutor;

        public async Task<TppApiObjectResponse<ListSlotsReply>> Post(
            (ListSlots listSlots, string suid) parameters)
        {
            var (listSlots, suid) = parameters;

            return await _requestExecutor.Post<ListSlotsReply>(
                requestBuilder => requestBuilder.Model(listSlots).Suid(suid));
        }
    }
}