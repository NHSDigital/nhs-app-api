using System.Threading.Tasks;

namespace NHSOnline.App.NhsLogin.Fido
{
    public interface IFidoKey
    {
        string KeyId();
        byte[] PublicKeyEccX962Raw();

        Task<byte[]> SignBytes(byte[] toSign);
    }
}