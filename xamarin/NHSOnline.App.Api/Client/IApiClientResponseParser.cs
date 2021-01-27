using System.Net.Http;
using System.Threading.Tasks;

namespace NHSOnline.App.Api.Client
{
    internal interface IApiClientResponseParser<TResult>
    {
        Task<TResult> Parse(HttpResponseMessage httpResponseMessage);
    }
}