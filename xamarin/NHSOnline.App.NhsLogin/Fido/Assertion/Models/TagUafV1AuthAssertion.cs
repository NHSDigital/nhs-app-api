namespace NHSOnline.App.NhsLogin.Fido.Assertion.Models
{
    /// <summary>
    /// https://fidoalliance.org/specs/fido-uaf-v1.2-rd-20171128/fido-uaf-authnr-cmds-v1.2-rd-20171128.html#tag_uafv1_auth_assertion
    /// </summary>
    internal sealed class TagUafV1AuthAssertion : ITagAssertion
    {
        private const ushort Tag = 0x3E02;

        internal TagUafV1SignedData UafV1SignedData { get; } = new TagUafV1SignedData();
        internal TagSignature Signature { get; } = new TagSignature();

        public void Write(ITagLengthValueWriter writer)
        {
            using var valueWriter = writer.StartTag(Tag);

            UafV1SignedData.Write(valueWriter);
            Signature.Write(valueWriter);
        }

        public override string ToString() => $"[{Tag:X4} {UafV1SignedData} {Signature}]";
    }
}