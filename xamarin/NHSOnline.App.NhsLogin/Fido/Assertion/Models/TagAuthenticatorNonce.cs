using System.Security.Cryptography;

namespace NHSOnline.App.NhsLogin.Fido.Assertion.Models
{
    internal sealed class TagAuthenticatorNonce
    {
        private readonly byte[] _nonce;
        private const ushort Tag = 0x2E0F;

        internal TagAuthenticatorNonce()
        {
            _nonce = new byte[8];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(_nonce);
        }

        internal void Write(ITagLengthValueWriter writer)
        {
            using var valueWriter = writer.StartTag(Tag);

            valueWriter.Write(_nonce);
        }

        public override string ToString() => $"[{Tag:X4} {_nonce.ToHexString()}]";
    }
}