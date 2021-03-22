using System;

namespace NHSOnline.App.NhsLogin.Fido.Assertion.Models
{
    internal sealed class TagSignature
    {
        private const ushort Tag = 0x2E06;

        private  byte[] _sigBytes = Array.Empty<byte>();

        internal void SigBytes(byte[] sigBytes) => _sigBytes = sigBytes;

        internal void Write(ITagLengthValueWriter writer)
        {
            using var valueWriter = writer.StartTag(Tag);

            valueWriter.Write(_sigBytes);
        }

        public override string ToString() => $"[{Tag:X4} {_sigBytes.ToHexString()}]";
    }
}