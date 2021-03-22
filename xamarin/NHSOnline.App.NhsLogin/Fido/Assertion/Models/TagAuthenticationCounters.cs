namespace NHSOnline.App.NhsLogin.Fido.Assertion.Models
{
    internal sealed class TagAuthenticationCounters
    {
        private const ushort Tag = 0x2E0D;

        private uint SignCounter { get; set; } = 1;

        internal void Write(ITagLengthValueWriter writer)
        {
            using var valueWriter = writer.StartTag(Tag);

            valueWriter.Write(SignCounter);
        }

        public override string ToString() => $"[{Tag:X4} {SignCounter:X8}]";
    }
}