using System.Threading.Tasks;
using NHSOnline.App.DependencyServices.Biometrics;

namespace NHSOnline.App.Services.FIDO
{
    internal interface IBiometricAuthenticationService
    {
        Task<BiometricRegisterResult> Register();
        Task<BiometricDeleteRegistrationResult> DeleteRegistration();
        Task<BiometricStatus?> FetchBiometricStatus();
    }
}