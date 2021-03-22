using System;
using System.Threading.Tasks;
using NHSOnline.App.DependencyServices.Biometrics;
using Security;

namespace NHSOnline.App.iOS.DependencyServices
{
    internal class BiometricAuthKey : IBiometricAuthKey
    {
        private readonly SecKey _secKey;

        public BiometricAuthKey(SecKey secKey)
        {
            _secKey = secKey;
        }
        
        public byte[] PublicKeyEccX962Raw()
        {
            using var data = _secKey.GetPublicKey().GetExternalRepresentation(out var error);
            try
            {
                if (data == null)
                {
                    throw new InvalidOperationException($"Failed to get external representation of key: {error}");
                }
                return data.ToArray();
            }
            finally
            {
                error?.Dispose();
            }
        }

        public Task<BiometricAuthVerifyUserResult> VerifyUser()
        {
            BiometricAuthVerifyUserResult result = new BiometricAuthVerifyUserResult.Authorised(new BiometricAuthSigner(_secKey));
            return Task.FromResult(result);
        }

        public Task Delete()
        {
            BiometricRegistrationDomainState.Clear();
            
            using var secRecord = new SecRecord(_secKey);
            SecKeyChain.Remove(secRecord);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _secKey.Dispose();
        }
    }
}