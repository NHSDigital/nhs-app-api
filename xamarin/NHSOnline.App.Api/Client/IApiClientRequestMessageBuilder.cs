using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NHSOnline.App.Api.Client
{
    internal interface IApiClientRequestMessageBuilder<TRequest>
    {
        Task Build(TRequest request, HttpRequestMessage httpRequestMessage, CookieContainer cookies);
    }
}