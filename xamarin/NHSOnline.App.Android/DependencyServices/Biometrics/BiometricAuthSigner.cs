using System.Threading.Tasks;
using AndroidX.Biometric;
using NHSOnline.App.DependencyServices.Biometrics;

namespace NHSOnline.App.Droid.DependencyServices.Biometrics
{
    internal sealed class BiometricAuthSigner : IBiometricAuthSigner
    {
        private readonly BiometricPrompt.CryptoObject _cryptoObject;

        public BiometricAuthSigner(BiometricPrompt.CryptoObject cryptoObject)
        {
            _cryptoObject = cryptoObject;
        }

        public Task<byte[]> SignBytes(byte[] toSign)
        {
            _cryptoObject.Signature.Update(toSign);
            var signature = _cryptoObject.Signature.Sign().ThrowIfNull("Signature is null");

            return Task.FromResult(signature);
        }
    }
}