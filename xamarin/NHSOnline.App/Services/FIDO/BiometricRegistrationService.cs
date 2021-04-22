using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.DependencyServices.Biometrics;
using NHSOnline.App.NhsLogin.Fido;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Services.FIDO
{
    internal sealed class BiometricRegistrationService
    {
        private readonly ILogger<BiometricRegistrationService> _logger;
        private readonly IBiometrics _biometrics;
        private readonly IFidoService _fidoService;
        private readonly IUserPreferencesService _preferencesService;

        public BiometricRegistrationService(
            ILogger<BiometricRegistrationService> logger,
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

            var authSigner = await VerifyUser(key).ResumeOnThreadPool();
            if (authSigner.Failed(out var authSignerFailure))
            {
                return authSignerFailure;
            }

            var keyId = await DoFidoRegistration(key, authSigner.Result, accessToken).ResumeOnThreadPool();
            if (keyId.Failed(out var keyIdFailure))
            {
                return keyIdFailure;
            }

            _preferencesService.BiometricsKeyId = keyId;
            return BiometricRegisterResult.Success();
        }

        public async Task DeleteRegistration(string accessToken)
        {
            var keyId = _preferencesService.BiometricsKeyId;
            _preferencesService.BiometricsKeyId = null;

            await DeleteAuthKey().ResumeOnThreadPool();
            await DeleteRegistration(accessToken, keyId).ResumeOnThreadPool();
        }

        private async Task<ProcessResult<string, BiometricRegisterResult>> DoFidoRegistration(
            IBiometricAuthKey key,
            IBiometricAuthSigner authSigner,
            string accessToken)
        {
            var keyId = GenerateRandomFidoKeyId();
            var fidoKey = new FidoKey(keyId, key, authSigner);
            var result = await _fidoService.Register(fidoKey, accessToken).ResumeOnThreadPool();
            return result.Accept(new FidoRegisterResultVisitor(keyId));
        }

        internal static string GenerateRandomFidoKeyId()
        {
            using var random = RandomNumberGenerator.Create();
            var bytes = new byte[64];
            random.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        private static async Task<ProcessResult<IBiometricAuthSigner, BiometricRegisterResult>> VerifyUser(IBiometricAuthKey key)
        {
            var verifyResult = await key.VerifyUser("To register with NHS login").ResumeOnThreadPool();

            var verifyUserResultVisitor = new RegisterVerifyUserResultVisitor();
            return verifyResult.Accept(verifyUserResultVisitor);
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

        private sealed class RegisterVerifyUserResultVisitor : IBiometricAuthVerifyUserResultVisitor<ProcessResult<IBiometricAuthSigner, BiometricRegisterResult>>
        {
            public ProcessResult<IBiometricAuthSigner, BiometricRegisterResult> Visit(BiometricAuthVerifyUserResult.Authorised authorised)
            {
                return ProcessResult<IBiometricAuthSigner, BiometricRegisterResult>.FromTResult(authorised.Signer);
            }

            public ProcessResult<IBiometricAuthSigner, BiometricRegisterResult> Visit(BiometricAuthVerifyUserResult.UserCancelled userCancelled)
            {
                return BiometricRegisterResult.UserCancelled();
            }

            public ProcessResult<IBiometricAuthSigner, BiometricRegisterResult> Visit(BiometricAuthVerifyUserResult.SystemCancelled systemCancelled)
            {
                return BiometricRegisterResult.SystemCancelled();
            }

            public ProcessResult<IBiometricAuthSigner, BiometricRegisterResult> Visit(BiometricAuthVerifyUserResult.Unauthorised unauthorised)
            {
                return BiometricRegisterResult.Failed(BiometricErrorCode.CannotChangeBiometrics);
            }

            public ProcessResult<IBiometricAuthSigner, BiometricRegisterResult> Visit(BiometricAuthVerifyUserResult.PermanentLockout permanentLockout)
            {
                return BiometricRegisterResult.Failed(BiometricErrorCode.CannotChangeBiometrics);
            }

            public ProcessResult<IBiometricAuthSigner, BiometricRegisterResult> Visit(BiometricAuthVerifyUserResult.TemporaryLockout temporaryLockout)
            {
                return BiometricRegisterResult.Failed(BiometricErrorCode.CannotChangeBiometrics);
            }
        }

        private sealed class FidoRegisterResultVisitor : IFidoRegisterResultVisitor<ProcessResult<string, BiometricRegisterResult>>
        {
            private readonly string _keyId;

            public FidoRegisterResultVisitor(string keyId)
            {
                _keyId = keyId;
            }

            public ProcessResult<string, BiometricRegisterResult> Visit(FidoRegisterResult.Registered registered)
            {
                return _keyId;
            }

            public ProcessResult<string, BiometricRegisterResult> Visit(FidoRegisterResult.Failed failed)
            {
                return BiometricRegisterResult.Failed(BiometricErrorCode.CannotChangeBiometrics);
            }
        }
    }
}