using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Api.Session;
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

        public async Task<BiometricRegisterResult> Register(AccessToken accessToken)
        {
            return await _biometricRegistrationService.Register(accessToken).PreserveThreadContext();
        }

        public async Task DeleteRegistration(AccessToken accessToken)
        {
            await _biometricRegistrationService.DeleteRegistration(accessToken).ResumeOnThreadPool();
        }

        public async Task<BiometricLoginResult> Authenticate(string fidoUsername)
        {
            return await _biometricLoginService.Authenticate(fidoUsername).PreserveThreadContext();
        }

        public async Task<BiometricStatusResult> FetchBiometricStatus(string fidoUsername)
        {
            var hasKeyId = _preferencesService.BiometricsKeyId != null;
            var biometricStatus = await _biometrics.FetchBiometricStatus(fidoUsername).ResumeOnThreadPool();

            return BiometricStatusResult.DeriveFrom(biometricStatus, hasKeyId);
        }

        public async Task DeleteRegistration(string fidoUsername)
        {
            await _biometricRegistrationService.DeleteRegistration(fidoUsername).ResumeOnThreadPool();
        }
    }
}
