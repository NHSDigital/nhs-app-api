using System;
using System.Threading.Tasks;
using Foundation;
using NHSOnline.App.DependencyServices.Biometrics;
using Security;

namespace NHSOnline.App.iOS.DependencyServices.Biometrics
{
    internal sealed class BiometricAuthSigner : IBiometricAuthSigner, IDisposable
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

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _secKey.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~BiometricAuthSigner()
        {
            Dispose(false);
        }
    }
}