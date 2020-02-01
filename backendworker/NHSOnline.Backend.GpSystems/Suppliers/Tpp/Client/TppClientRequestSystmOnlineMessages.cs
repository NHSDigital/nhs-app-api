using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientRequestSystmOnlineMessages
        : ITppClientRequest<(RequestSystmOnlineMessages requestModel, string suid), RequestSystmOnlineMessagesReply>
    {
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientRequestSystmOnlineMessages(TppClientRequestExecutor requestExecutor)
            => _requestExecutor = requestExecutor;

        public async Task<TppApiObjectResponse<RequestSystmOnlineMessagesReply>> Post(
            (RequestSystmOnlineMessages requestModel, string suid) parameters)
        {
            var (requestModel, suid) = parameters;

            return await _requestExecutor.Post<RequestSystmOnlineMessagesReply>(
                requestBuilder => requestBuilder.Model(requestModel).Suid(suid));
        }
    }
}