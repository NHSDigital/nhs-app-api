namespace NHSOnline.App.NhsLogin.Fido.Assertion.Models
{
    internal sealed class TagUafV1SignedData
    {
        private const ushort Tag = 0x3E04;

        internal TagAuthenticatorAttestationId AuthenticatorAttestationId { get; } = new TagAuthenticatorAttestationId();
        internal TagAuthenticateAssertionInfo AssertionInfo { get; } = new TagAuthenticateAssertionInfo();
        internal TagAuthenticatorNonce AuthenticatorNonce { get; } = new TagAuthenticatorNonce();
        internal TagFinalChallenge FinalChallenge { get; } = new TagFinalChallenge();
        internal TagTransactionContent TransactionContent { get; } = new TagTransactionContent();
        internal TagKeyId KeyId { get; } = new TagKeyId();
        internal TagAuthenticationCounters Counters { get; } = new TagAuthenticationCounters();

        internal void Write(ITagLengthValueWriter writer)
        {
            using var valueWriter = writer.StartTag(Tag);

            AuthenticatorAttestationId.Write(valueWriter);
            AssertionInfo.Write(valueWriter);
            AuthenticatorNonce.Write(valueWriter);
            FinalChallenge.Write(valueWriter);
            TransactionContent.Write(valueWriter);
            KeyId.Write(valueWriter);
            Counters.Write(valueWriter);
        }

        public override string ToString() => $"[{Tag:X4} {AuthenticatorAttestationId} {AssertionInfo} {AuthenticatorNonce} {FinalChallenge} {TransactionContent} {KeyId} {Counters}]";
    }
}