using System;

namespace NHSOnline.App.NhsLogin.Fido.Assertion.Models
{
    internal sealed class TagPubKey
    {
        private const ushort Tag = 0x2E0C;

        private byte[] _keyBytes = Array.Empty<byte>();

        internal void KeyBytes(byte[] keyBytes) => _keyBytes = keyBytes;

        internal void Write(ITagLengthValueWriter writer)
        {
            using var valueWriter = writer.StartTag(Tag);

            valueWriter.Write(_keyBytes);
        }

        public override string ToString() => $"[{Tag:X4} {_keyBytes.ToHexString()}]";
    }
}