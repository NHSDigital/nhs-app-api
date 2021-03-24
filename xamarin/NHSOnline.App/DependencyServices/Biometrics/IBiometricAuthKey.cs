using System;
using System.Threading.Tasks;

namespace NHSOnline.App.DependencyServices.Biometrics
{
    public interface IBiometricAuthKey: IDisposable
    {
        byte[] PublicKeyEccX962Raw();

        Task<BiometricAuthVerifyUserResult> VerifyUser(string reason);
        Task Delete();
    }
}