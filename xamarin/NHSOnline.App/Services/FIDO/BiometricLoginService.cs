using System;
using System.Threading.Tasks;
using NHSOnline.App.DependencyServices.Biometrics;
using NHSOnline.App.NhsLogin.Fido;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Services.FIDO
{
    internal sealed class BiometricLoginService
    {
        private readonly IBiometrics _biometrics;
        private readonly IFidoService _fidoService;
        private readonly IUserPreferencesService _preferencesService;

        public BiometricLoginService(
            IBiometrics biometrics,
            IFidoService fidoService,
            IUserPreferencesService preferencesService)
        {
            _biometrics = biometrics;
            _fidoService = fidoService;
            _preferencesService = preferencesService;
        }

        public async Task<BiometricLoginResult> Authenticate()
        {
            var keyId = await ValidateRegistration().ResumeOnThreadPool();
            if (keyId.Failed(out var keyIdFailure))
            {
                return keyIdFailure;
            }

            if (!_biometrics.TryGetKey(out var biometricAuthKey))
            {
                return new BiometricLoginResult.Invalidated();
            }

            var authSigner = await VerifyUser(biometricAuthKey).ResumeOnThreadPool();
            if (authSigner.Failed(out var authSignerFailure))
            {
                return authSignerFailure;
            }

            return await DoFidoLogin(keyId, biometricAuthKey, authSigner.Result).ResumeOnThreadPool();
        }

        private async Task<ProcessResult<string, BiometricLoginResult>> ValidateRegistration()
        {
            var keyId = _preferencesService.BiometricsKeyId;
            if (keyId is null)
            {
                return new BiometricLoginResult.NotRegistered();
            }

            var status = await _biometrics.FetchBiometricStatus().ResumeOnThreadPool();

            return status.Accept(new BiometricCanLoginResultVisitor(keyId));
        }

        private static async Task<ProcessResult<IBiometricAuthSigner, BiometricLoginResult>> VerifyUser(IBiometricAuthKey key)
        {
            var verifyResult = await key.VerifyUser("Log in to NHS App").ResumeOnThreadPool();

            return verifyResult.Accept(new LoginVerifyUserResultVisitor());
        }

        private async Task<BiometricLoginResult> DoFidoLogin(string keyId, IBiometricAuthKey key, IBiometricAuthSigner signer)
        {
            var result = await _fidoService.Authorise(new FidoKey(keyId, key, signer)).ResumeOnThreadPool();
            return result.Accept(new FidoAuthorisationResultVisitor());
        }

        private sealed class LoginVerifyUserResultVisitor : IBiometricAuthVerifyUserResultVisitor<ProcessResult<IBiometricAuthSigner, BiometricLoginResult>>
        {
            public ProcessResult<IBiometricAuthSigner, BiometricLoginResult> Visit(BiometricAuthVerifyUserResult.Authorised authorised)
            {
                return ProcessResult<IBiometricAuthSigner, BiometricLoginResult>.FromTResult(authorised.Signer);
            }

            public ProcessResult<IBiometricAuthSigner, BiometricLoginResult> Visit(BiometricAuthVerifyUserResult.Cancelled cancelled)
            {
                return new BiometricLoginResult.Cancelled();
            }

            public ProcessResult<IBiometricAuthSigner, BiometricLoginResult> Visit(BiometricAuthVerifyUserResult.Unauthorised unauthorised)
            {
                return new BiometricLoginResult.Failed();
            }

            public ProcessResult<IBiometricAuthSigner, BiometricLoginResult> Visit(BiometricAuthVerifyUserResult.LockedOut lockedOut)
            {
                return new BiometricLoginResult.Invalidated();
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
                return new BiometricLoginResult.NotRegistered();
            }

            public ProcessResult<string, BiometricLoginResult> Visit(BiometricStatus.FingerPrint fingerPrint)
            {
                return GetCanLoginResult(fingerPrint.RegistrationStatus);
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
                    BiometricRegistrationStatus.NotRegistered => new BiometricLoginResult.NotRegistered(),
                    BiometricRegistrationStatus.Registered => _keyId,
                    BiometricRegistrationStatus.Invalidated => new BiometricLoginResult.Invalidated(),
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
                return new BiometricLoginResult.Unauthorised();
            }

            public BiometricLoginResult Visit(FidoAuthorisationResult.Failed failed)
            {
                return new BiometricLoginResult.Failed();
            }
        }
    }
}