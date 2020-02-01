using System.Net.Http;
using System.Threading.Tasks;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal sealed class TppClientRequestSerializer : ITppClientRequestSender
    {
        private readonly TppClientRequestLock _requestLock;
        private readonly TppClientRequestSender _requestSender;

        public TppClientRequestSerializer(TppClientRequestLock requestLock, TppClientRequestSender requestSender)
        {
            _requestLock = requestLock;
            _requestSender = requestSender;
        }

        public async Task<TppApiObjectResponse<TResponse>> SendRequestAndParseResponse<TResponse>(HttpRequestMessage request)
        {
            using (await _requestLock.Acquire())
            {
                return await _requestSender.SendRequestAndParseResponse<TResponse>(request);
            }
        }
    }
}