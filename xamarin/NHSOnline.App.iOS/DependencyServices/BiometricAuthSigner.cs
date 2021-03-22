using System;
using System.Threading.Tasks;
using Foundation;
using NHSOnline.App.DependencyServices.Biometrics;
using Security;

namespace NHSOnline.App.iOS.DependencyServices
{
    internal sealed class BiometricAuthSigner : IBiometricAuthSigner
    {
        private readonly SecKey _secKey;

        public BiometricAuthSigner(SecKey secKey)
        {
            _secKey = secKey;
        }

        public Task<byte[]> SignBytes(byte[] toSign)
        {
            using var dataToSign = NSData.FromArray(toSign);
            using var signature = _secKey.CreateSignature(SecKeyAlgorithm.EcdsaSignatureMessageX962Sha256, dataToSign, out var error);
            try
            {
                if (signature == null)
                {
                    throw new InvalidOperationException($"Failed to sign data: {error}");
                }
                return Task.FromResult(signature.ToArray());
            }
            finally
            {
                error?.Dispose();
            }
        }
    }
}