using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.BinaryData;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientRequestBinaryDataPost :
        ITppClientRequest<(TppRequestParameters tppRequestParameters, string documentIdentifier), RequestBinaryDataReply>
    {
        private readonly TppClientRequestExecutor _requestExecutor;

        public TppClientRequestBinaryDataPost(TppClientRequestExecutor requestExecutor)
        {
            _requestExecutor = requestExecutor;
        }

        public async Task<TppApiObjectResponse<RequestBinaryDataReply>> Post(
            (TppRequestParameters tppRequestParameters, string documentIdentifier) parameters)
        {
            var (tppRequestParameters, documentIdentifier) = parameters;
            var binaryDataRequest = new RequestBinaryData
            {
                PatientId = tppRequestParameters.PatientId,
                OnlineUserId = tppRequestParameters.OnlineUserId,
                UnitId = tppRequestParameters.OdsCode,
                BinaryDataId = documentIdentifier
            };

            return await _requestExecutor.Post<RequestBinaryDataReply>(
                requestBuilder => requestBuilder.Model(binaryDataRequest).Suid(tppRequestParameters.Suid));
        }
    }
}