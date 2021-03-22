namespace NHSOnline.App.NhsLogin.Fido.Assertion.Models
{
    /// <summary>
    /// https://fidoalliance.org/specs/fido-uaf-v1.2-rd-20171128/fido-uaf-authnr-cmds-v1.2-rd-20171128.html#tag_uafv1_reg_assertion
    /// </summary>
    internal sealed class TagUafV1RegAssertion : ITagAssertion
    {
        private const ushort Tag = 0x3E01;

        internal TagUafV1KeyRegistrationData UafV1KeyRegistrationData { get; } = new TagUafV1KeyRegistrationData();
        internal TagAttestationBasicFull AttestationBasicFull { get; } = new TagAttestationBasicFull();

        public void Write(ITagLengthValueWriter writer)
        {
            using var valueWriter = writer.StartTag(Tag);

            UafV1KeyRegistrationData.Write(valueWriter);
            AttestationBasicFull.Write(valueWriter);
        }

        public override string ToString() => $"[{Tag:X4} {UafV1KeyRegistrationData} {AttestationBasicFull}]";
    }
}