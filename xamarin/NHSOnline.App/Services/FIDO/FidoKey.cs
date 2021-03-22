using System.Text;
using System.Threading.Tasks;
using NHSOnline.App.DependencyServices.Biometrics;
using NHSOnline.App.NhsLogin.Fido;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Services.FIDO
{
    internal sealed class FidoKey : IFidoKey
    {
        private readonly byte[] _keyId;
        private readonly IBiometricAuthKey _key;
        private readonly IBiometricAuthSigner _signer;

        public FidoKey(string keyId, IBiometricAuthKey key, IBiometricAuthSigner signer)
        {
            _keyId = Encoding.UTF8.GetBytes(keyId);
            _key = key;
            _signer = signer;
        }

        public byte[] KeyId() => _keyId;

        public byte[] PublicKeyEccX962Raw() => _key.PublicKeyEccX962Raw();

        public async Task<byte[]> SignBytes(byte[] toSign) => await _signer.SignBytes(toSign).ResumeOnThreadPool();
    }
}