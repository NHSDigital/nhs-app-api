using System;

namespace NHSOnline.App.NhsLogin.Fido.Assertion.Models
{
    internal sealed class TagFinalChallenge
    {
        private const ushort Tag = 0x2E0A;

        private byte[] _hash = Array.Empty<byte>();

        internal void Hash(byte[] hash) => _hash = hash;

        internal void Write(ITagLengthValueWriter writer)
        {
            using var valueWriter = writer.StartTag(Tag);

            valueWriter.Write(_hash);
        }

        public override string ToString() => $"[{Tag:X4} {_hash.ToHexString()}]";
    }
}