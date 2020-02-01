using System.Net.Http;
using System.Threading.Tasks;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal interface ITppClientRequestSender
    {
        Task<TppApiObjectResponse<TResponse>> SendRequestAndParseResponse<TResponse>(HttpRequestMessage request);
    }
}