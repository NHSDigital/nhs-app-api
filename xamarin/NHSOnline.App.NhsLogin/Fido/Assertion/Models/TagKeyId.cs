namespace NHSOnline.App.NhsLogin.Fido.Assertion.Models
{
    internal sealed class TagKeyId
    {
        private const ushort Tag = 0x2E09;

        internal string Value { get; set; } = string.Empty;

        internal void Write(ITagLengthValueWriter writer)
        {
            using var valueWriter = writer.StartTag(Tag);

            valueWriter.Write(Value);
        }

        public override string ToString() => $"[{Tag:X4} {Value}]";
    }
}