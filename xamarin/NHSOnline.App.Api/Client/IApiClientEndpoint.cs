using System.Threading.Tasks;

namespace NHSOnline.App.Api.Client
{
    internal interface IApiClientEndpoint<TRequest, TResult>
    {
        Task<TResult> Call(TRequest request);
    }
}
