using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.BinaryData;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientRequestBinaryDataPost
        : ITppClientRequest<(RequestBinaryData requestBinaryData, TppUserSession userSession), RequestBinaryDataReply>
    {
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientRequestBinaryDataPost(
            TppClientRequestExecutor requestExecutor)
        {
            _requestExecutor = requestExecutor;
        }

        public async Task<TppApiObjectResponse<RequestBinaryDataReply>> Post(
            (RequestBinaryData requestBinaryData, TppUserSession userSession) parameters)
        {
            var (requestBinaryData, userSession) = parameters;
            return await _requestExecutor.Post<RequestBinaryDataReply>(
                requestBuilder => requestBuilder.Model(requestBinaryData).Suid(userSession.Suid));
        }
    }
}