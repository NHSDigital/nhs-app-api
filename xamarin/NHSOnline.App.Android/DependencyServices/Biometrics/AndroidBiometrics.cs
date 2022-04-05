using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Security.Keystore;
using AndroidX.Biometric;
using AndroidX.Fragment.App;
using Java.Lang;
using Java.Security;
using Java.Security.Spec;
using Microsoft.Extensions.Logging;
using NHSOnline.App.DependencyServices.Biometrics;
using NHSOnline.App.Droid.DependencyServices.Biometrics;
using NHSOnline.App.Logging;
using Xamarin.Forms;
using Exception = System.Exception;

[assembly: Dependency(typeof(AndroidBiometrics))]
namespace NHSOnline.App.Droid.DependencyServices.Biometrics
{
    public sealed class AndroidBiometrics : IBiometrics
    {
        internal static Activity MainActivity { get; set; } = null!;
        private static ILogger Logger => NhsAppLogging.CreateLogger<AndroidBiometrics>();

        public Task<BiometricStatus> FetchBiometricStatus(string fidoUsername)
        {
            using var manager = BiometricManager.From(MainActivity);

            BiometricStatus.FingerPrintFaceOrIris Unusable() =>
                new BiometricStatus.FingerPrintFaceOrIris(BiometricHardwareState.Unusable,
                    DeriveBiometricRegistrationStatus(fidoUsername));

            BiometricStatus.FingerPrintFaceOrIris Usable() =>
                new BiometricStatus.FingerPrintFaceOrIris(BiometricHardwareState.Usable,
                    DeriveBiometricRegistrationStatus(fidoUsername));

            BiometricStatus.HardwareNotPresent HardwareNotPresent() => new BiometricStatus.HardwareNotPresent();

            BiometricStatus.LegacySensorNotValid LegacySensorNotValid() => new BiometricStatus.LegacySensorNotValid();

            try
            {
                var canAuthenticate = manager.CanAuthenticate(BiometricManager.Authenticators.BiometricStrong);

                BiometricStatus status = canAuthenticate switch
                {
                    BiometricManager.BiometricErrorHwUnavailable => Unusable(),
                    BiometricManager.BiometricErrorNoneEnrolled => Unusable(),
                    BiometricManager.BiometricErrorNoHardware => LegacySensorNotValid(),
                    BiometricManager.BiometricErrorSecurityUpdateRequired => Unusable(),
                    BiometricManager.BiometricErrorUnsupported => LegacySensorNotValid(),
                    BiometricManager.BiometricStatusUnknown => Unusable(),
                    BiometricManager.BiometricSuccess => Usable(),
                    _ => HardwareNotPresent()
                };

                return Task.FromResult(status);
            }
            catch (IllegalStateException e) when (e.Message != null && e.Message.Contains("Unknown security", StringComparison.Ordinal))
            {
                Logger.LogError(e, "Illegal state exception fetching biometric status - Unknown security level");

                // Return unusable if we cannot determine the devices security level
                return Task.FromResult((BiometricStatus) Unusable());
            }

        }

        private BiometricRegistrationStatus DeriveBiometricRegistrationStatus(string fidoUsername)
        {
            var registered = BiometricRegistrationState.FidoRegistered;
            if (!registered)
            {
                return BiometricRegistrationStatus.NotRegistered;
            }

            try
            {
                if (TryGetKey(fidoUsername, out _))
                {
                    return BiometricRegistrationStatus.Registered;
                }
            }
            catch (CrossPlatformException e) when (e.ErrorType is CrossPlatformErrorType.UnrecoverableKey)
            {
                Logger.LogError(e, "Unable to get key - Registration invalid");
            }

            return BiometricRegistrationStatus.Invalidated;
        }

        public Task<IBiometricAuthKey> CreateBiometricKey(string fidoUsername)
        {
            var keyPairGenerator = KeyPairGenerator
                .GetInstance(KeyProperties.KeyAlgorithmEc, BiometricKeyStore.InstanceName)
                .ThrowIfNull("KeyPairGenerator is null");
            var keyGenParameterSpec = BuildKeyGenParameterSpec(fidoUsername);

            try
            {
                keyPairGenerator.Initialize(keyGenParameterSpec);
            }
            catch (GeneralSecurityException e) when (e.Message != null && e.Message.Contains("must be enrolled to create keys requiring user authentication for every", StringComparison.Ordinal))
            {
                using var keyguardManager = (KeyguardManager?)Android.App.Application.Context.GetSystemService("keyguard");

                if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                {
                    var isDeviceSecure = keyguardManager?.IsDeviceSecure == true;

                    Logger.LogError(
                        "KeyguardManager reported IsDeviceSecure as: {IsDeviceSecure}. Original error message: {Message}",
                        isDeviceSecure, e.Message);
                }

                throw;
            }

            try
            {
                _ = keyPairGenerator.GenerateKeyPair() ??
                    throw new InvalidOperationException("GenerateKeyPair returns null");
            }
            catch (ProviderException e)
                when (e.Message != null && e.Message.Contains("Keystore not initialized", StringComparison.Ordinal))
            {
                // Try this operation once more in case it is purely a timing issue in loading the KeyStore
                Task.Delay(1000);

                _ = keyPairGenerator.GenerateKeyPair() ??
                    throw new InvalidOperationException("GenerateKeyPair returns null");
            }
            catch (ProviderException e)
            {
                Logger.LogError(e, "ProviderException when trying to generate keypair");
                throw new InvalidOperationException("GenerateKeyPair failed");
            }

            try
            {
                if (TryGetKey(fidoUsername, out var biometricAuthKey))
                {
                    BiometricRegistrationState.FidoRegistered = true;
                    return Task.FromResult(biometricAuthKey);
                }
            }
            catch (CrossPlatformException e) when (e.ErrorType is CrossPlatformErrorType.UnrecoverableKey)
            {
                Logger.LogError(e, "Unable to create key - Unrecoverable key exception");
                throw;
            }

            throw new InvalidOperationException("GenerateKeyPair failed");
        }

        private static KeyGenParameterSpec BuildKeyGenParameterSpec(string fidoUsername)
        {
            var keystoreAlias = $"{BiometricKeyStore.KeyPrefix}_{fidoUsername}";

            using var builder = new KeyGenParameterSpec.Builder(keystoreAlias, KeyStorePurpose.Sign);
            using var ecGenParameterSpec = new ECGenParameterSpec("secp256r1");

            builder
                .SetAlgorithmParameterSpec(ecGenParameterSpec)
                ?.SetDigests(
                    KeyProperties.DigestSha256,
                    KeyProperties.DigestSha384,
                    KeyProperties.DigestSha512)
                .SetUserAuthenticationRequired(true);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                builder.SetAttestationChallenge(Array.Empty<byte>());
                builder.SetInvalidatedByBiometricEnrollment(true);
            }

            return builder.Build();
        }

        public bool TryGetKey(string fidoUsername, [NotNullWhen(true)] out IBiometricAuthKey? key)
        {
            try
            {
                var keyStoreResult = AccessKeyStoreAndRetrieveKey();
                key = keyStoreResult.biometricAuthKey;
                return keyStoreResult.keyLoaded;
            }
            catch (UnrecoverableKeyException e)
                when (e.Message != null && e.Message.Contains("Keystore not initialized", StringComparison.Ordinal))
            {
                // Try this operation once more in case it is purely a timing issue in loading the KeyStore
                Task.Delay(1000);

                var keyStoreResult = AccessKeyStoreAndRetrieveKey();
                key = keyStoreResult.biometricAuthKey;
                return keyStoreResult.keyLoaded;
            }
            catch (UnrecoverableKeyException e)
            {
                Logger.LogError(e, "Failed to get Biometric Auth Key");
                throw new CrossPlatformException("UnrecoverableKeyException", CrossPlatformErrorType.UnrecoverableKey);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to get Biometric Auth Key");
            }

            key = default;
            return false;

            (bool keyLoaded, IBiometricAuthKey? biometricAuthKey) AccessKeyStoreAndRetrieveKey()
            {
                var secretKey = BiometricKeyStore.GetKey(fidoUsername);
                var certificate = BiometricKeyStore.GetCertificate(fidoUsername);

                if (secretKey != null && certificate != null)
                {
                    using var keyStoreResult = new BiometricAuthKey((FragmentActivity)MainActivity, secretKey, certificate);
                    return new (true, keyStoreResult);
                }

                return new (false, null);
            }
        }
    }
}