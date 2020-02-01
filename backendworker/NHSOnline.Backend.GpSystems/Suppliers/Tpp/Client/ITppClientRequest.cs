using System.Threading.Tasks;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    internal interface ITppClientRequest<TParams, TReply>
    {
        Task<TppApiObjectResponse<TReply>> Post(TParams parameters);
    }
}