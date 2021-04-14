using System.Threading.Tasks;

namespace NHSOnline.App.Services.FIDO
{
    internal interface IBiometricAuthenticationService
    {
        Task<BiometricStatusResult> FetchBiometricStatus();
        Task<BiometricRegisterResult> Register(string accessToken);
        Task DeleteRegistration(string accessToken);
        Task<BiometricLoginResult> Authenticate();
    }
}