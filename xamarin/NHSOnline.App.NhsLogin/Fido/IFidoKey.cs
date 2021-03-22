using System.Threading.Tasks;

namespace NHSOnline.App.NhsLogin.Fido
{
    public interface IFidoKey
    {
        byte[] KeyId();
        byte[] PublicKeyEccX962Raw();

        Task<byte[]> SignBytes(byte[] toSign);
    }
}