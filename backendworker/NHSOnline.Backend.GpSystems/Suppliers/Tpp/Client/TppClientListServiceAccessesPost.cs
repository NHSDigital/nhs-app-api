using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Services;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientListServiceAccessesPost : ITppClientRequest<TppUserSession, ListServiceAccessesReply>
    {
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientListServiceAccessesPost(TppClientRequestExecutor requestExecutor)
            => _requestExecutor = requestExecutor;

        public async Task<TppApiObjectResponse<ListServiceAccessesReply>> Post(TppUserSession tppUserSession)
        {
            var listServiceAccesses = new ListServiceAccesses
            {
                PatientId = tppUserSession.PatientId,
                OnlineUserId = tppUserSession.OnlineUserId,
                UnitId = tppUserSession.OdsCode
            };

            return await _requestExecutor.Post<ListServiceAccessesReply>(
                requestBuilder => requestBuilder.Model(listServiceAccesses).Suid(tppUserSession.Suid));
        }
    }
}