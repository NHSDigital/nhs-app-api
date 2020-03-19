using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.BinaryData;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientRequestBinaryDataPost :
        ITppClientRequest<(TppUserSession userSession, string documentIdentifier), RequestBinaryDataReply>
    {
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientRequestBinaryDataPost(TppClientRequestExecutor requestExecutor)
        {
            _requestExecutor = requestExecutor;
        }

        public async Task<TppApiObjectResponse<RequestBinaryDataReply>> Post(
            ( TppUserSession userSession, string documentIdentifier) parameters)
        {
            var (userSession, documentIdentifier) = parameters;
            var binaryDataRequest = new RequestBinaryData
            {
                PatientId = userSession.PatientId,
                OnlineUserId = userSession.OnlineUserId,
                UnitId = userSession.UnitId,
                BinaryDataId = documentIdentifier
            };

            return await _requestExecutor.Post<RequestBinaryDataReply>(
                requestBuilder => requestBuilder.Model(binaryDataRequest).Suid(userSession.Suid));
        }
    }
}