using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.DependencyServices.Biometrics;
using NHSOnline.App.NhsLogin.Fido;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Services.FIDO
{
    internal sealed class BiometricAuthenticationService : IBiometricAuthenticationService
    {
        private readonly ILogger _logger;
        private readonly IBiometrics _biometrics;
        private readonly IFidoService _fidoService;
        private readonly IUserPreferencesService _preferencesService;

        public BiometricAuthenticationService(
            ILogger<BiometricAuthenticationService> logger,
            IBiometrics biometrics,
            IFidoService fidoService,
            IUserPreferencesService preferencesService)
        {
            _logger = logger;
            _biometrics = biometrics;
            _fidoService = fidoService;
            _preferencesService = preferencesService;
        }

        public async Task<BiometricRegisterResult> Register(string accessToken)
        {
            await DeleteRegistration(accessToken).ResumeOnThreadPool();

            using var key = await _biometrics.CreateBiometricKey().ResumeOnThreadPool();
            var verifyResult = await key.VerifyUser().ResumeOnThreadPool();

            var verifyUserResultVisitor = new RegisterVerifyUserResultVisitor(_preferencesService, _fidoService, key, accessToken);
            return await verifyResult.Accept(verifyUserResultVisitor).ResumeOnThreadPool();
        }

        public async Task DeleteRegistration(string accessToken)
        {
            var keyId = _preferencesService.BiometricsKeyId;
            _preferencesService.BiometricsKeyId = null;

            await DeleteAuthKey().ResumeOnThreadPool();
            await DeleteRegistration(accessToken, keyId).ResumeOnThreadPool();
        }

        public async Task<BiometricStatusResult> FetchBiometricStatus()
        {
            var hasKeyId = _preferencesService.BiometricsKeyId != null;
            var biometricStatus = await _biometrics.FetchBiometricStatus().ResumeOnThreadPool();

            return BiometricStatusResult.DeriveFrom(biometricStatus, hasKeyId);
        }

        private static string GenerateKeyId()
        {
            using var random = RandomNumberGenerator.Create();
            var bytes = new byte[64];
            random.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        private async Task DeleteAuthKey()
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

        private async Task DeleteRegistration(string accessToken, string? keyId)
        {
            try
            {
                if (keyId != null)
                {
                    await _fidoService.Deregister(accessToken, keyId).ResumeOnThreadPool();
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Failed to delete registration");
            }
        }

        private sealed class RegisterVerifyUserResultVisitor : IBiometricAuthVerifyUserResultVisitor<Task<BiometricRegisterResult>>
        {
            private readonly IUserPreferencesService _preferencesService;
            private readonly IFidoService _fidoService;
            private readonly IBiometricAuthKey _key;
            private readonly string _accessToken;

            public RegisterVerifyUserResultVisitor(
                IUserPreferencesService preferencesService,
                IFidoService fidoService,
                IBiometricAuthKey key,
                string accessToken)
            {
                _preferencesService = preferencesService;
                _fidoService = fidoService;
                _key = key;
                _accessToken = accessToken;
            }

            public async Task<BiometricRegisterResult> Visit(BiometricAuthVerifyUserResult.Authorised authorised)
            {
                var keyId = GenerateKeyId();

                var fidoKey = new FidoKey(keyId, _key, authorised.Signer);
                var result = await _fidoService.Register(fidoKey, _accessToken).ResumeOnThreadPool();
                return result.Accept(new FidoRegisterResultVisitor(_preferencesService, keyId));
            }

            public Task<BiometricRegisterResult> Visit(BiometricAuthVerifyUserResult.Cancelled cancelled)
            {
                return Task.FromResult(BiometricRegisterResult.Cancelled());
            }

            public Task<BiometricRegisterResult> Visit(BiometricAuthVerifyUserResult.Failed failed)
            {
                return Task.FromResult(BiometricRegisterResult.Failed(BiometricErrorCode.Unknown));
            }
        }

        private sealed class FidoRegisterResultVisitor : IFidoRegisterResultVisitor<BiometricRegisterResult>
        {
            private readonly IUserPreferencesService _preferencesService;
            private readonly string _keyId;

            public FidoRegisterResultVisitor(
                IUserPreferencesService preferencesService,
                string keyId)
            {
                _preferencesService = preferencesService;
                _keyId = keyId;
            }

            public BiometricRegisterResult Visit(FidoRegisterResult.Registered registered)
            {
                _preferencesService.BiometricsKeyId = _keyId;
                return BiometricRegisterResult.Success();
            }

            public BiometricRegisterResult Visit(FidoRegisterResult.Failed failed)
            {
                return BiometricRegisterResult.Failed(BiometricErrorCode.Unknown);
            }
        }
    }
}
