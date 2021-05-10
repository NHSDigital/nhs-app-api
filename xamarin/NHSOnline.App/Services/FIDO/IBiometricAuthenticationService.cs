using System.Threading.Tasks;
using NHSOnline.App.Api.Session;

namespace NHSOnline.App.Services.FIDO
{
    internal interface IBiometricAuthenticationService
    {
        Task<BiometricStatusResult> FetchBiometricStatus(string fidoUsername);
        Task<BiometricRegisterResult> Register(AccessToken accessToken);
        Task DeleteRegistration(AccessToken accessToken);
        Task DeleteRegistration(string fidoUserName);
        Task<BiometricLoginResult> Authenticate(string fidoUsername);
    }
}