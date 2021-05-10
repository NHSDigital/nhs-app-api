using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Security.Keystore;
using AndroidX.Biometric;
using AndroidX.Fragment.App;
using Java.Security;
using Java.Security.Spec;
using NHSOnline.App.DependencyServices.Biometrics;
using NHSOnline.App.Droid.DependencyServices.Biometrics;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidBiometrics))]
namespace NHSOnline.App.Droid.DependencyServices.Biometrics
{
    public sealed class AndroidBiometrics : IBiometrics
    {
        internal static Activity MainActivity { get; set; } = null!;

        public Task<BiometricStatus> FetchBiometricStatus(string fidoUsername)
        {
            using var manager = BiometricManager.From(MainActivity);

            var canAuthenticate = manager.CanAuthenticate(BiometricManager.Authenticators.BiometricWeak);

            BiometricStatus status = canAuthenticate switch
            {
                BiometricManager.BiometricErrorHwUnavailable => Unusable(),
                BiometricManager.BiometricErrorNoneEnrolled => Unusable(),
                BiometricManager.BiometricErrorNoHardware => HardwareNotPresent(),
                BiometricManager.BiometricErrorSecurityUpdateRequired => Unusable(),
                BiometricManager.BiometricErrorUnsupported => HardwareNotPresent(),
                BiometricManager.BiometricStatusUnknown => Unusable(),
                BiometricManager.BiometricSuccess => Usable(),
                _ => HardwareNotPresent()
            };

            return Task.FromResult(status);

            BiometricStatus.FingerPrint Unusable() => new BiometricStatus.FingerPrint(BiometricHardwareState.Unusable, DeriveBiometricRegistrationStatus(fidoUsername));

            BiometricStatus.FingerPrint Usable() => new BiometricStatus.FingerPrint(BiometricHardwareState.Usable, DeriveBiometricRegistrationStatus(fidoUsername));

            BiometricStatus.HardwareNotPresent HardwareNotPresent() => new BiometricStatus.HardwareNotPresent();
        }

        private BiometricRegistrationStatus DeriveBiometricRegistrationStatus(string fidoUsername)
        {
            var registered = BiometricRegistrationState.FidoRegistered;
            if (!registered)
            {
                return BiometricRegistrationStatus.NotRegistered;
            }

            if (TryGetKey(fidoUsername, out _))
            {
                return BiometricRegistrationStatus.Registered;
            }

            return BiometricRegistrationStatus.Invalidated;
        }

        public Task<IBiometricAuthKey> CreateBiometricKey(string fidoUsername)
        {
            var keyPairGenerator = KeyPairGenerator
                .GetInstance(KeyProperties.KeyAlgorithmEc, BiometricKeyStore.InstanceName)
                .ThrowIfNull("KeyPairGenerator is null");
            var keyGenParameterSpec = BuildKeyGenParameterSpec(fidoUsername);

            keyPairGenerator.Initialize(keyGenParameterSpec);

            _ = keyPairGenerator.GenerateKeyPair() ?? throw new InvalidOperationException("GenerateKeyPair returns null");
            if (TryGetKey(fidoUsername, out var biometricAuthKey))
            {
                BiometricRegistrationState.FidoRegistered = true;
                return Task.FromResult(biometricAuthKey);
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
            }

            return builder.Build();
        }

        public bool TryGetKey(string fidoUsername, [NotNullWhen(true)] out IBiometricAuthKey? key)
        {
            var secretKey = BiometricKeyStore.GetKey(fidoUsername);
            var certificate = BiometricKeyStore.GetCertificate(fidoUsername);

            if (secretKey != null && certificate != null)
            {
                key = new BiometricAuthKey((FragmentActivity)MainActivity, secretKey, certificate);
                return true;
            }

            key = default;
            return false;
        }
    }
}