using System;
using System.Threading.Tasks;
using Foundation;
using LocalAuthentication;
using Microsoft.Extensions.Logging;
using NHSOnline.App.DependencyServices.Biometrics;
using NHSOnline.App.Logging;
using NHSOnline.App.Threading;
using Security;

namespace NHSOnline.App.iOS.DependencyServices.Biometrics
{
    internal class BiometricAuthKey : IBiometricAuthKey
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger<BiometricAuthKey>();

        private readonly LAContext _context;
        private readonly SecKey _secKey;

        public BiometricAuthKey(LAContext context, SecKey secKey)
        {
            _context = context;
            _secKey = secKey;
        }

        public byte[] PublicKeyEccX962Raw()
        {
            NSError? error = new NSError();
            using var data = _secKey.GetPublicKey()?.GetExternalRepresentation(out error);
            try
            {
                if (data == null)
                {
                    throw new InvalidOperationException($"Failed to get external representation of key: {error}");
                }

                return data.ToArray();
            }
            finally
            {
                error?.Dispose();
            }
        }

        private static string BiometricPromptText(VerificationReason verificationReason)
        {
            return verificationReason switch
            {
                VerificationReason.Login => "Log in with Touch ID",
                VerificationReason.Registration => "Turn on Touch ID",
                _ => throw new ArgumentOutOfRangeException(nameof(verificationReason), verificationReason, null)
            };
        }

        public async Task<BiometricAuthVerifyUserResult> VerifyUser(VerificationReason verificationReason)
        {
            // Remove "Enter Password" option from prompt when first biometric attempt fails
            _context.LocalizedFallbackTitle = string.Empty;

            var (success, error) = await _context
                .EvaluatePolicyAsync(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, BiometricPromptText(verificationReason))
                .ResumeOnThreadPool();

            try
            {
                if (success)
                {
                    if (BiometricRegistrationDomainState.Get() is null &&
                        _context.EvaluatedPolicyDomainState != null)
                    {
                        BiometricRegistrationDomainState.Set(_context.EvaluatedPolicyDomainState);
                    }

                    return new BiometricAuthVerifyUserResult.Authorised(new BiometricAuthSigner(_secKey));
                }

                var status = (LAStatus?) (long?) error?.Code;

                if (status == LAStatus.BiometryLockout)
                {
                    Logger.LogInformation("EvaluatePolicyAsync locked out");
                    return new BiometricAuthVerifyUserResult.PermanentLockout();
                }

                if (status == LAStatus.UserCancel)
                {
                    Logger.LogInformation("EvaluatePolicyAsync canceled by user");
                    return new BiometricAuthVerifyUserResult.UserCancelled();
                }

                if (status == LAStatus.SystemCancel)
                {
                    Logger.LogInformation("EvaluatePolicyAsync canceled by system");
                    return new BiometricAuthVerifyUserResult.SystemCancelled();
                }

                Logger.LogWarning("EvaluatePolicyAsync failed. Error: {Error} and status: {Status}", error, status);
                return new BiometricAuthVerifyUserResult.Unauthorised();
            }
            finally
            {
                error?.Dispose();
            }
        }

        public Task Delete(string fidoUsername)
        {
            BiometricRegistrationDomainState.Clear();

            using var secRecord = new SecRecord(_secKey);
            SecKeyChain.Remove(secRecord);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _secKey.Dispose();
            _context.Dispose();
        }
    }
}