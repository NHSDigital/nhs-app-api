using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Foundation;
using LocalAuthentication;
using NHSOnline.App.DependencyServices.Biometrics;
using Security;

namespace NHSOnline.App.iOS.DependencyServices.Biometrics
{
    internal class BiometricAuthKeyProvider : IBiometricAuthKeyProvider
    {
        private const string PrivateKeyLabel = "nhs-biometrics-key";

        public Task<IBiometricAuthKey> CreateBiometricKey()
        {
            var keyGenerationParameters = CreateGenerationParameters();

            CreateKey(keyGenerationParameters);

            if (TryGetKey(out var biometricAuthKey))
            {
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
                    KeyClass = SecKeyClass.Private,
                    AuthenticationContext = context
                };

                var queryResult = SecKeyChain.QueryAsConcreteType(query, out _);

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

        private static SecKeyGenerationParameters CreateGenerationParameters()
        {
            var flags = Compatibility.MinimumRequiredVersion(11, 3)
                ? SecAccessControlCreateFlags.BiometryCurrentSet
                : SecAccessControlCreateFlags.TouchIDCurrentSet;

            using var accessControl = new SecAccessControl(
                SecAccessible.WhenUnlockedThisDeviceOnly,
                flags | SecAccessControlCreateFlags.PrivateKeyUsage);

            var keyGenerationParameters = new SecKeyGenerationParameters
            {
                KeyType = SecKeyType.ECSecPrimeRandom,
                KeySizeInBits = 256,
                Label = PrivateKeyLabel,
                TokenID = SecTokenID.SecureEnclave,
                PrivateKeyAttrs = new SecKeyParameters
                {
                    IsPermanent = true,
                    AccessControl = accessControl
                }
            };
            return keyGenerationParameters;
        }

        private static void CreateKey(SecKeyGenerationParameters keyGenerationParameters)
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
            finally
            {
                key?.Dispose();
                error?.Dispose();
            }
        }
    }
}