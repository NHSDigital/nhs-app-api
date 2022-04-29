using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.DependencyServices.Biometrics;
using NHSOnline.App.Logging;
using NHSOnline.App.NhsLogin.Fido;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Services.FIDO
{
    internal sealed class BiometricLoginService
    {
        private readonly IBiometrics _biometrics;
        private readonly IFidoService _fidoService;
        private readonly IUserPreferencesService _preferencesService;
        private readonly ILogger _logger;

        public BiometricLoginService(
            IBiometrics biometrics,
            IFidoService fidoService,
            IUserPreferencesService preferencesService,
            ILogger<BiometricLoginService> logger)
        {
            _biometrics = biometrics;
            _fidoService = fidoService;
            _preferencesService = preferencesService;
            _logger = logger;
        }

        public async Task<BiometricLoginResult> Authenticate(string fidoUsername)
        {
            var keyId = await ValidateRegistration(fidoUsername).ResumeOnThreadPool();
            if (keyId.Failed(out var keyIdFailure))
            {
                return keyIdFailure;
            }

            try
            {
                if (!_biometrics.TryGetKey(fidoUsername, out var biometricAuthKey))
                {
                    return new BiometricLoginResult.Lockout(LockoutType.Permanent);
                }

                var authSigner = await VerifyUser(biometricAuthKey).PreserveThreadContext();
                if (authSigner.Failed(out var authSignerFailure))
                {
                    return authSignerFailure;
                }

                return await DoFidoLogin(keyId, biometricAuthKey, authSigner.Result).ResumeOnThreadPool();
            }
            catch (CrossPlatformException e) when (e.ErrorType is CrossPlatformErrorType.UnrecoverableKey)
            {
                _logger.LogError(e, "Unrecoverable key exception");
                return new BiometricLoginResult.Lockout(LockoutType.Permanent);
            }
        }

        private async Task<ProcessResult<string, BiometricLoginResult>> ValidateRegistration(string fidoUsername)
        {
            var keyId = _preferencesService.BiometricsKeyId;
            if (keyId is null)
            {
                return new BiometricLoginResult.NoAction(NoActionReason.NotRegistered);
            }

            var status = await _biometrics.FetchBiometricStatus(fidoUsername).ResumeOnThreadPool();

            return status.Accept(new BiometricCanLoginResultVisitor(keyId));
        }

        private static async Task<ProcessResult<IBiometricAuthSigner, BiometricLoginResult>> VerifyUser(IBiometricAuthKey key)
        {
            var verifyResult = await key.VerifyUser(VerificationReason.Login).PreserveThreadContext();

            return verifyResult.Accept(new LoginVerifyUserResultVisitor());
        }

        private async Task<BiometricLoginResult> DoFidoLogin(string keyId, IBiometricAuthKey key, IBiometricAuthSigner signer)
        {
            var result = await _fidoService.Authorise(new FidoKey(keyId, key, signer)).PreserveThreadContext();
            return result.Accept(new FidoAuthorisationResultVisitor());
        }

        private sealed class LoginVerifyUserResultVisitor : IBiometricAuthVerifyUserResultVisitor<ProcessResult<IBiometricAuthSigner, BiometricLoginResult>>
        {
            public ProcessResult<IBiometricAuthSigner, BiometricLoginResult> Visit(BiometricAuthVerifyUserResult.Authorised authorised)
            {
                return ProcessResult<IBiometricAuthSigner, BiometricLoginResult>.FromTResult(authorised.Signer);
            }

            public ProcessResult<IBiometricAuthSigner, BiometricLoginResult> Visit(BiometricAuthVerifyUserResult.UserCancelled userCancelled)
            {
                return new BiometricLoginResult.NoAction(NoActionReason.UserCancelled);
            }

            public ProcessResult<IBiometricAuthSigner, BiometricLoginResult> Visit(BiometricAuthVerifyUserResult.SystemCancelled systemCancelled)
            {
                return new BiometricLoginResult.CouldNotLogin(CouldNotLoginReason.SystemCancelled);
            }

            public ProcessResult<IBiometricAuthSigner, BiometricLoginResult> Visit(BiometricAuthVerifyUserResult.Unauthorised unauthorised)
            {
                return new BiometricLoginResult.Failed();
            }

            public ProcessResult<IBiometricAuthSigner, BiometricLoginResult> Visit(BiometricAuthVerifyUserResult.PermanentLockout permanentLockout)
            {
                return new BiometricLoginResult.Lockout(LockoutType.Permanent);
            }

            public ProcessResult<IBiometricAuthSigner, BiometricLoginResult> Visit(BiometricAuthVerifyUserResult.TemporaryLockout temporaryLockout)
            {
                return new BiometricLoginResult.Lockout(LockoutType.Temporary);
            }

            public ProcessResult<IBiometricAuthSigner, BiometricLoginResult> Visit(BiometricAuthVerifyUserResult.VendorError vendorError)
            {
                return new BiometricLoginResult.NoAction(NoActionReason.VendorError);
            }
        }

        private sealed class BiometricCanLoginResultVisitor : IBiometricStatusVisitor<ProcessResult<string, BiometricLoginResult>>
        {
            private readonly string _keyId;

            public BiometricCanLoginResultVisitor(string keyId)
            {
                _keyId = keyId;
            }

            public ProcessResult<string, BiometricLoginResult> Visit(BiometricStatus.HardwareNotPresent hardwareNotPresent)
            {
                return new BiometricLoginResult.NoAction(NoActionReason.NotRegistered);
            }

            public ProcessResult<string, BiometricLoginResult> Visit(BiometricStatus.LegacySensorNotValid legacySensorNotValid)
            {
                return new BiometricLoginResult.LegacySensorNotValid();
            }

            public ProcessResult<string, BiometricLoginResult> Visit(BiometricStatus.FingerPrintFaceOrIris fingerPrintFaceOrIris)
            {
                return GetCanLoginResult(fingerPrintFaceOrIris.RegistrationStatus);
            }

            public ProcessResult<string, BiometricLoginResult> Visit(BiometricStatus.TouchId touchId)
            {
                return GetCanLoginResult(touchId.RegistrationStatus);
            }

            public ProcessResult<string, BiometricLoginResult> Visit(BiometricStatus.FaceId faceId)
            {
                return GetCanLoginResult(faceId.RegistrationStatus);
            }

            private ProcessResult<string, BiometricLoginResult> GetCanLoginResult(BiometricRegistrationStatus status)
            {
                return status switch
                {
                    BiometricRegistrationStatus.NotRegistered => new BiometricLoginResult.NoAction(NoActionReason.NotRegistered),
                    BiometricRegistrationStatus.Registered => _keyId,
                    BiometricRegistrationStatus.Invalidated => new BiometricLoginResult.Lockout(LockoutType.Permanent),
                    _ => throw new InvalidOperationException($"Unknown registration status: {status}")
                };
            }
        }

        private sealed class FidoAuthorisationResultVisitor : IFidoAuthorisationResultVisitor<BiometricLoginResult>
        {
            public BiometricLoginResult Visit(FidoAuthorisationResult.Authorised authorised)
            {
                return new BiometricLoginResult.Authorised(authorised.FidoAuthResponse);
            }

            public BiometricLoginResult Visit(FidoAuthorisationResult.Unauthorised unauthorised)
            {
                return new BiometricLoginResult.CouldNotLogin(CouldNotLoginReason.Unauthorised);
            }

            public BiometricLoginResult Visit(FidoAuthorisationResult.PermanentLockout permanentLockout)
            {
                return new BiometricLoginResult.Lockout(LockoutType.Permanent);
            }
        }
    }
}