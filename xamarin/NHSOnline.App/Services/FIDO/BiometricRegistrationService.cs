using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Api.Session;
using NHSOnline.App.DependencyServices.Biometrics;
using NHSOnline.App.Logging;
using NHSOnline.App.NhsLogin.Fido;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Services.FIDO
{
    internal sealed class BiometricRegistrationService
    {
        private readonly ILogger<BiometricRegistrationService> _logger;
        private readonly IBiometrics _biometrics;
        private readonly IFidoService _fidoService;
        private readonly IUserPreferencesService _userPreferencesService;

        public BiometricRegistrationService(
            ILogger<BiometricRegistrationService> logger,
            IBiometrics biometrics,
            IFidoService fidoService,
            IUserPreferencesService userPreferencesService)
        {
            _logger = logger;
            _biometrics = biometrics;
            _fidoService = fidoService;
            _userPreferencesService = userPreferencesService;
        }

        public async Task<BiometricRegisterResult> Register(AccessToken accessToken)
        {
            await DeleteRegistration(accessToken).PreserveThreadContext();

            try
            {
                using var key = await _biometrics.CreateBiometricKey(accessToken.Subject).PreserveThreadContext();

                var authSigner = await VerifyUser(key).PreserveThreadContext();
                if (authSigner.Failed(out var authSignerFailure))
                {
                    return authSignerFailure;
                }

                var keyId = await DoFidoRegistration(key, authSigner.Result, accessToken).ResumeOnThreadPool();
                if (keyId.Failed(out var keyIdFailure))
                {
                    return keyIdFailure;
                }

                _userPreferencesService.BiometricsKeyId = keyId;
                return BiometricRegisterResult.Success();
            }
            catch (CrossPlatformException)
            {
                return BiometricRegisterResult.Failed(BiometricErrorCode.CannotChangeBiometrics);
            }
        }

        public async Task DeleteRegistration(AccessToken accessToken)
        {
            var keyId = _userPreferencesService.BiometricsKeyId;

            _userPreferencesService.BiometricsKeyId = null;
            _userPreferencesService.FidoUsername = string.Empty;

            await DeleteAuthKey(accessToken.Subject).ResumeOnThreadPool();
            await DeleteRegistration(accessToken, keyId).ResumeOnThreadPool();
        }

        public async Task DeleteRegistration(string fidoUserName)
        {
            var keyId = _userPreferencesService.BiometricsKeyId;

            _userPreferencesService.BiometricsKeyId = null;
            _userPreferencesService.FidoUsername = string.Empty;

            await DeleteAuthKey(fidoUserName).ResumeOnThreadPool();
        }

        private async Task<ProcessResult<string, BiometricRegisterResult>> DoFidoRegistration(
            IBiometricAuthKey key,
            IBiometricAuthSigner authSigner,
            AccessToken accessToken)
        {
            var keyId = GenerateRandomFidoKeyId();
            var fidoKey = new FidoKey(keyId, key, authSigner);
            var result = await _fidoService.Register(fidoKey, accessToken.Raw()).ResumeOnThreadPool();
            return result.Accept(new FidoRegisterResultVisitor(keyId, _userPreferencesService));
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
            var verifyResult = await key.VerifyUser(VerificationReason.Registration).PreserveThreadContext();

            var verifyUserResultVisitor = new RegisterVerifyUserResultVisitor();
            return verifyResult.Accept(verifyUserResultVisitor);
        }

        private async Task DeleteAuthKey(string fidoUsername)
        {
            try
            {
                if (_biometrics.TryGetKey(fidoUsername, out var authKey))
                {
                    await authKey.Delete(fidoUsername).ResumeOnThreadPool();
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Failed to delete auth key");
            }
        }

        private async Task DeleteRegistration(AccessToken accessToken, string? keyId)
        {
            try
            {
                if (keyId != null)
                {
                    await _fidoService.Deregister(accessToken.Raw(), keyId).ResumeOnThreadPool();
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
            private readonly IUserPreferencesService _userPreferencesService;

            public FidoRegisterResultVisitor(string keyId, IUserPreferencesService userPreferencesService)
            {
                _keyId = keyId;
                _userPreferencesService = userPreferencesService;
            }

            public ProcessResult<string, BiometricRegisterResult> Visit(FidoRegisterResult.Registered registered)
            {
                _userPreferencesService.FidoUsername = registered.Username;
                return _keyId;
            }

            public ProcessResult<string, BiometricRegisterResult> Visit(FidoRegisterResult.Failed failed)
            {
                return BiometricRegisterResult.Failed(BiometricErrorCode.CannotChangeBiometrics);
            }
        }
    }
}