using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Util;
using AndroidX.Biometric;
using NHSOnline.App.DependencyServices.Biometrics;
using NHSOnline.App.Logging;

namespace NHSOnline.App.Droid.DependencyServices.Biometrics
{
    internal sealed class BiometricAuthSigner : IBiometricAuthSigner
    {
        private readonly BiometricPrompt.CryptoObject _cryptoObject;

        private static readonly List<string> UnrecoverableAndroidKeyStoreExceptions = new List<string>
        {
            // This exception wrapper covers a multitude of android.security.KeyStoreExceptions which are raised by 'KeyStore2.java' class.
            "java.security.UnrecoverableKeyException",
            // The following KeyStoreExceptions do not get wrapped in the Java one above, but we will handle in the same way.
            "android.security.KeyStoreException: Key user not authenticated"
        };

        public BiometricAuthSigner(BiometricPrompt.CryptoObject cryptoObject)
        {
            _cryptoObject = cryptoObject;
        }

        public Task<byte[]> SignBytes(byte[] toSign)
        {
            try
            {
                _cryptoObject.Signature.Update(toSign);
                var signature = _cryptoObject.Signature.Sign().ThrowIfNull("Signature is null");

                return Task.FromResult(signature);
            }
            catch (Exception e) when (IsUnrecoverableAndroidKeyStoreException(e))
            {
                Log.Error(nameof(BiometricAuthSigner), $"{nameof(SignBytes)}: Unrecoverable key exception");
                throw new CrossPlatformException($"{nameof(SignBytes)}: Unrecoverable key exception - ${e.Message}",
                    CrossPlatformErrorType.UnrecoverableKey);
            }
        }

        private static bool IsUnrecoverableAndroidKeyStoreException(Exception exception) =>
            UnrecoverableAndroidKeyStoreExceptions.Any(knownKeyStoreExceptionStringToMatch =>
                exception.ToString().Contains(knownKeyStoreExceptionStringToMatch, StringComparison.Ordinal));
    }
}