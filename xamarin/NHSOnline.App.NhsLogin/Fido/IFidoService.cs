using System.Threading.Tasks;

namespace NHSOnline.App.NhsLogin.Fido
{
    public interface IFidoService
    {
        Task<FidoRegisterResult> Register(IFidoKey key, string accessToken);
        Task Deregister(string accessToken, string keyId);
        Task<FidoAuthorisationResult> Authorise(IFidoKey fidoKey);
    }
}