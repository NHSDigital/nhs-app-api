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
        private const string PrivateKeyLabel = "nhs-biometrics-key";

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

        [SuppressMessage("ReSharper", "CA2000", Justification = "BiomtericAuthKey is not ready to be disposed of")]
        public Task<IBiometricAuthKey> CreateBiometricKey()
        {
            using var context = new LAContext();
            var supported = BiometricsHardware.HasDeviceOwnerPermittedUseOfBiometricHardware(context, out var state, out var enrolled);
            if (!supported || state == BiometricHardwareState.Unusable || context.EvaluatedPolicyDomainState is null)
            {
                throw new InvalidOperationException("Cannot create auth key: Biometrics unusable");
            }

            using var secKey = CreateKey();

            using var secRecord = new SecRecord(SecKind.Key)
            {
                Label = PrivateKeyLabel,
                AuthenticationContext = context
            };
            secRecord.SetKey(secKey);

            SecKeyChain.Add(secRecord);

            if (TryGetKey(out var biometricAuthKey))
            {
                BiometricRegistrationDomainState.Set(context.EvaluatedPolicyDomainState);

                return Task.FromResult(biometricAuthKey);
            }

            throw new InvalidOperationException("Failed to retrieve generated key");
        }

        public bool TryGetKey([NotNullWhen(true)] out IBiometricAuthKey? key)
        {
            var context = new LAContext();
            try
            {
                using var query = new SecRecord(SecKind.Key)
                {
                    Label = PrivateKeyLabel,
                    AuthenticationContext = context
                };

                var queryResult = SecKeyChain.QueryAsConcreteType(query, out var secStatusCode);

                Console.WriteLine(secStatusCode);

                if (queryResult is SecKey secKey)
                {
                    key = new BiometricAuthKey(context, secKey);
                    return true;
                }

                key = null;
                return false;
            }
            catch
            {
                context.Dispose();
                throw;
            }
        }
    }
}
#endif