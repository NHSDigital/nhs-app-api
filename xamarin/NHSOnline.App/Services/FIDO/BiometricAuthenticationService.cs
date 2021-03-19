using System;
using System.Threading.Tasks;
using NHSOnline.App.DependencyServices.Biometrics;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Services.FIDO
{
    internal sealed class BiometricAuthenticationService : IBiometricAuthenticationService
    {
        private readonly IBiometrics _biometrics;

        public BiometricAuthenticationService(IBiometrics biometrics)
        {
            _biometrics = biometrics;
        }

        public async Task<BiometricRegisterResult> Register()
        {
            await _biometrics.FetchBiometricStatus().ResumeOnThreadPool();
            return BiometricRegisterResult.Failed(BiometricErrorCode.CannotFindBiometrics);
        }

        public Task<BiometricDeleteRegistrationResult> DeleteRegistration()
        {
            throw new NotImplementedException();
        }

        public async Task<BiometricStatus?> FetchBiometricStatus()
        {
            return await _biometrics.FetchBiometricStatus().ResumeOnThreadPool();
        }
    }
}
