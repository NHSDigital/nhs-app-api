namespace NHSOnline.App.NhsLogin.Fido.Assertion.Models
{
    internal sealed class TagRegistrationCounters
    {
        private const ushort Tag = 0x2E0D;

        private uint SignCounter { get; set; } = 1;
        private uint RegCounter { get; set; } = 1;

        internal void Write(ITagLengthValueWriter writer)
        {
            using var valueWriter = writer.StartTag(Tag);

            valueWriter.Write(SignCounter);
            valueWriter.Write(RegCounter);
        }

        public override string ToString() => $"[{Tag:X4} {SignCounter:X8} {RegCounter:X8}]";
    }
}