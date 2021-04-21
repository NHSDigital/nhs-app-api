using System;
using System.Threading.Tasks;
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
            using var data = _secKey.GetPublicKey().GetExternalRepresentation(out var error);
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

        public async Task<BiometricAuthVerifyUserResult> VerifyUser(string reason)
        {
            // Remove "Enter Password" option from prompt when first biometric attempt fails
            _context.LocalizedFallbackTitle = string.Empty;

            var (success, error) = await _context
                .EvaluatePolicyAsync(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, reason)
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
                    return new BiometricAuthVerifyUserResult.LockedOut();
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

                Logger.LogWarning("EvaluatePolicyAsync failed: {Error}", error);
                return new BiometricAuthVerifyUserResult.Unauthorised();
            }
            finally
            {
                error?.Dispose();
            }
        }

        public Task Delete()
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