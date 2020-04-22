using System;
using System.Threading.Tasks;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal interface ITppClientRequestExecutor
    {
        Task<TppApiObjectResponse<TReply>> Post<TReply>(Action<ITppClientRequestBuilder> buildRequest);
    }
}
