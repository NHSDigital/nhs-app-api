namespace NHSOnline.App.NhsLogin.Fido.Assertion.Models
{
    internal sealed class TagAttestationBasicFull
    {
        private const ushort Tag = 0x3E07;

        internal TagSignature Signature { get; } = new TagSignature();

        internal void Write(ITagLengthValueWriter writer)
        {
            using var valueWriter = writer.StartTag(Tag);

            Signature.Write(valueWriter);
        }

        public override string ToString() => $"[{Tag:X} {Signature}]";
    }
}