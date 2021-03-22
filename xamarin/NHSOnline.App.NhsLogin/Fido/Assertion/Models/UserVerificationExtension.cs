namespace NHSOnline.App.NhsLogin.Fido.Assertion.Models
{
    /// <summary>
    /// https://fidoalliance.org/specs/fido-uaf-v1.2-rd-20171128/fido-uaf-reg-v1.2-rd-20171128.html#user-verification-method-extension
    /// </summary>
    internal sealed class UserVerificationExtension
    {
        private const ushort Tag = 0x3E11;

        internal UserVerificationExtensionId Id { get; } = new UserVerificationExtensionId();
        internal UserVerificationExtensionData Data { get; } = new UserVerificationExtensionData();

        internal void Write(ITagLengthValueWriter writer)
        {
            using var valueWriter = writer.StartTag(Tag);

            Id.Write(valueWriter);
            Data.Write(valueWriter);
        }

        public override string ToString() => $"[{Tag:X4} {Id} {Data}]";
    }
}