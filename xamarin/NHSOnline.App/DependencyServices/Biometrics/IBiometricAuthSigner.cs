using System.Threading.Tasks;

namespace NHSOnline.App.DependencyServices.Biometrics
{
    public interface IBiometricAuthSigner
    {
        Task<byte[]> SignBytes(byte[] toSign);
    }
}