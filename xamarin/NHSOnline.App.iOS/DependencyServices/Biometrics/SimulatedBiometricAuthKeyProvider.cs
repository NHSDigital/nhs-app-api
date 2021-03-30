#if SIMULATOR
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using LocalAuthentication;
using NHSOnline.App.DependencyServices.Biometrics;
using Security;

namespace NHSOnline.App.iOS.DependencyServices.Biometrics
{
    internal class SimulatedBiometricAuthKeyProvider : IBiometricAuthKeyProvider
    {
        private SecKey? _secKey;

        private static SecKey CreateKey()
        {
            SecKey.GenerateKeyPair(
                SecKeyType.ECSecPrimeRandom,
                256,
                new SecPublicPrivateKeyAttrs(),
                out var publicKey,
                out var privateKey);
            publicKey?.Dispose();

            return privateKey!;
        }

        public Task<IBiometricAuthKey> CreateBiometricKey()
        {
            using var context = new LAContext();
            var supported = BiometricsHardware.HasBiometricHardware(context, out var state);
            if (!supported || state == BiometricHardwareState.Unusable || context.EvaluatedPolicyDomainState is null)
            {
                throw new InvalidOperationException("Cannot create auth key: Biometrics unusable");
            }

            _secKey ??= CreateKey();

            BiometricRegistrationDomainState.Set(context.EvaluatedPolicyDomainState);

            return Task.FromResult<IBiometricAuthKey>(new BiometricAuthKey(new LAContext(),_secKey));
        }

        public bool TryGetKey([NotNullWhen(true)] out IBiometricAuthKey? key)
        {
            if (_secKey == null)
            {
                key = null;
                return false;
            }

            key = new BiometricAuthKey(new LAContext(), _secKey);
            return true;
        }
    }
}
#endif