using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientRequestSystmOnlineMessages
        : ITppClientRequest<TppRequestParameters, RequestSystmOnlineMessagesReply>
    {
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientRequestSystmOnlineMessages(TppClientRequestExecutor requestExecutor)
            => _requestExecutor = requestExecutor;

        public async Task<TppApiObjectResponse<RequestSystmOnlineMessagesReply>> Post(TppRequestParameters tppRequestParameters)
        {
            var requestModel = new RequestSystmOnlineMessages(tppRequestParameters);

            return await _requestExecutor.Post<RequestSystmOnlineMessagesReply>(
                requestBuilder => requestBuilder.Model(requestModel).Suid(tppRequestParameters.Suid));
        }
    }
}