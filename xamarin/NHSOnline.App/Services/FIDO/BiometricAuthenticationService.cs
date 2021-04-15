using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.DependencyServices.Biometrics;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Services.FIDO
{
    internal sealed class BiometricAuthenticationService : IBiometricAuthenticationService
    {
        private readonly ILogger _logger;
        private readonly IBiometrics _biometrics;
        private readonly IUserPreferencesService _preferencesService;
        private readonly BiometricRegistrationService _biometricRegistrationService;
        private readonly BiometricLoginService _biometricLoginService;

        public BiometricAuthenticationService(
            ILogger<BiometricAuthenticationService> logger,
            IBiometrics biometrics,
            IUserPreferencesService preferencesService,
            BiometricRegistrationService biometricRegistrationService,
            BiometricLoginService biometricLoginService)
        {
            _logger = logger;
            _biometrics = biometrics;
            _preferencesService = preferencesService;
            _biometricRegistrationService = biometricRegistrationService;
            _biometricLoginService = biometricLoginService;
        }

        public async Task<BiometricRegisterResult> Register(string accessToken)
        {
            return await _biometricRegistrationService.Register(accessToken).ResumeOnThreadPool();
        }

        public async Task DeleteRegistration(string accessToken)
        {
            await _biometricRegistrationService.DeleteRegistration(accessToken).ResumeOnThreadPool();
        }

        public async Task<BiometricLoginResult> Authenticate()
        {
            return await _biometricLoginService.Authenticate().ResumeOnThreadPool();
        }

        public async Task<BiometricStatusResult> FetchBiometricStatus()
        {
            var hasKeyId = _preferencesService.BiometricsKeyId != null;
            var biometricStatus = await _biometrics.FetchBiometricStatus().ResumeOnThreadPool();

            return BiometricStatusResult.DeriveFrom(biometricStatus, hasKeyId);
        }

        public async Task DeleteAuthKey()
        {
            try
            {
                if (_biometrics.TryGetKey(out var authKey))
                {
                    await authKey.Delete().ResumeOnThreadPool();
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Failed to delete auth key");
            }
        }
    }
}
