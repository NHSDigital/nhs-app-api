using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Foundation;
using LocalAuthentication;
using Microsoft.Extensions.Logging;
using NHSOnline.App.DependencyServices.Biometrics;
using NHSOnline.App.iOS.DependencyServices;
using NHSOnline.App.Logging;
using Security;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(IosBiometrics))]
namespace NHSOnline.App.iOS.DependencyServices
{
    public sealed class IosBiometrics : IBiometrics
    {
        private const string PrivateKeyLabel = "nhs-biometrics-key";

        private static ILogger Logger => NhsAppLogging.CreateLogger<IosBiometrics>();

        public Task<BiometricStatus> FetchBiometricStatus()
        {
            BiometricStatus status = new BiometricStatus.HardwareNotPresent();

            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                using var context = new LAContext();
                if (HasBiometricHardware(context, out var state))
                {
                    var registrationStatus = DeriveRegistrationStatus(context);
                    if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
                    {
                        status = context.BiometryType switch
                        {
                            LABiometryType.FaceId => new BiometricStatus.FaceId(state, registrationStatus),
                            _ => new BiometricStatus.TouchId(state, registrationStatus)
                        };
                    }
                    else
                    {
                        status = new BiometricStatus.TouchId(state, registrationStatus);
                    }
                }
            }

            return Task.FromResult(status);
        }

        public Task<IBiometricAuthKey> CreateBiometricKey()
        {
            using var context = new LAContext();
            var supported = HasBiometricHardware(context, out var state);
            if (!supported || state == BiometricHardwareState.Unusable || context.EvaluatedPolicyDomainState is null)
            {
                throw new InvalidOperationException("Cannot create auth key: Biometrics unusable");
            }

            var keyGenerationParameters = CreateGenerationParameters();

            var key = CreateKey(keyGenerationParameters);

            BiometricRegistrationDomainState.Set(context.EvaluatedPolicyDomainState);

            return Task.FromResult<IBiometricAuthKey>(new BiometricAuthKey(key));
        }

        public bool TryGetKey([NotNullWhen(true)] out IBiometricAuthKey? key)
        {
            using var query = new SecRecord(SecKind.Key)
            {
                Label = PrivateKeyLabel,
                KeyClass = SecKeyClass.Private
            };

            var queryResult = SecKeyChain.QueryAsConcreteType(query, out _);

            if (queryResult is SecKey secKey)
            {
                key = new BiometricAuthKey(secKey);
                return true;
            }

            key = null;
            return false;
        }

        private static BiometricRegistrationStatus DeriveRegistrationStatus(LAContext context)
        {
            using var registeredDomainState = BiometricRegistrationDomainState.Get();

            if (registeredDomainState is null)
            {
                return BiometricRegistrationStatus.NotRegistered;
            }

            if (registeredDomainState.Equals(context.EvaluatedPolicyDomainState))
            {
                return BiometricRegistrationStatus.Registered;
            }

            return BiometricRegistrationStatus.Invalidated;
        }

        private static SecKeyGenerationParameters CreateGenerationParameters()
        {
            var flags = UIDevice.CurrentDevice.CheckSystemVersion(11, 3)
                ? SecAccessControlCreateFlags.BiometryCurrentSet
                : SecAccessControlCreateFlags.TouchIDCurrentSet;

            using var accessControl = new SecAccessControl(
                SecAccessible.WhenUnlockedThisDeviceOnly,
                flags | SecAccessControlCreateFlags.PrivateKeyUsage);

            var keyGenerationParameters = new SecKeyGenerationParameters
            {
                KeyType = SecKeyType.EC,
                KeySizeInBits = 256,
                Label = PrivateKeyLabel,
                TokenID = SecTokenID.SecureEnclave,
                PrivateKeyAttrs = new SecKeyParameters
                {
                    IsPermanent = true,
#if !SIMULATOR
                    AccessControl = accessControl
#endif
                }
            };

#if SIMULATOR
            keyGenerationParameters.TokenID = SecTokenID.None;
#endif
            return keyGenerationParameters;
        }

        private static bool HasBiometricHardware(LAContext context, out BiometricHardwareState state)
        {
            NSError? error = null;
            try
            {
                var enabled = context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out error);
                if (enabled)
                {
                    state = BiometricHardwareState.Usable;
                    return true;
                }
                
                Logger.LogInformation("CanEvaluatePolicy returned error: {Error}", error);

                state = BiometricHardwareState.Unusable;

                return (LAStatus?)(long?)error?.Code switch
                {
                    LAStatus.BiometryNotEnrolled => true,
                    LAStatus.BiometryLockout => true,
                    LAStatus.BiometryNotAvailable => false,
                    _ => false
                };
            }
            finally
            {
                error?.Dispose();
            }
        }

        [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller is responsible for calling Dispose")]
        private static SecKey CreateKey(SecKeyGenerationParameters keyGenerationParameters)
        {
            SecKey? key = null;
            NSError? error = null;
            try
            {
                key = SecKey.CreateRandomKey(keyGenerationParameters, out error);
                if (error != null)
                {
                    throw new InvalidOperationException($"{nameof(SecKey.CreateRandomKey)} Failed: {error}");
                }

                if (key is null)
                {
                    throw new InvalidOperationException($"{nameof(SecKey.CreateRandomKey)} Returned null");
                }
            }
            catch (Exception)
            {
                key?.Dispose();
                throw;
            }
            finally
            {
                error?.Dispose();
            }

            return key;
        }
    }
}