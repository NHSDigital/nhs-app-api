using System.Threading.Tasks;
using NHSOnline.App.DependencyServices.Biometrics;
using NHSOnline.App.NhsLogin.Fido;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Services.FIDO
{
    internal sealed class FidoKey : IFidoKey
    {
        private readonly string _keyId;
        private readonly IBiometricAuthKey _key;
        private readonly IBiometricAuthSigner _signer;

        public FidoKey(string keyId, IBiometricAuthKey key, IBiometricAuthSigner signer)
        {
            _keyId = keyId;
            _key = key;
            _signer = signer;
        }

        string IFidoKey.KeyId() => _keyId;

        byte[] IFidoKey.PublicKeyEccX962Raw() => _key.PublicKeyEccX962Raw();

        async Task<byte[]> IFidoKey.SignBytes(byte[] toSign) => await _signer.SignBytes(toSign).PreserveThreadContext();
    }
}