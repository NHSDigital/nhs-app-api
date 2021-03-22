namespace NHSOnline.App.NhsLogin.Fido.Assertion.Models
{
    internal sealed class TagUafV1KeyRegistrationData
    {
        private const ushort Tag = 0x3E03;

        internal TagAuthenticatorAttestationId AaId { get; } = new TagAuthenticatorAttestationId();
        internal TagRegisterAssertionInfo AssertionInfo { get; } = new TagRegisterAssertionInfo();
        internal TagFinalChallenge FinalChallenge { get; } = new TagFinalChallenge();
        internal TagKeyId KeyId { get; } = new TagKeyId();
        internal TagRegistrationCounters Counters { get; } = new TagRegistrationCounters();
        internal TagPubKey PubKey { get; } = new TagPubKey();

        internal void Write(ITagLengthValueWriter writer)
        {
            using var valueWriter = writer.StartTag(Tag);

            AaId.Write(valueWriter);
            AssertionInfo.Write(valueWriter);
            FinalChallenge.Write(valueWriter);
            KeyId.Write(valueWriter);
            Counters.Write(valueWriter);
            PubKey.Write(valueWriter);
        }

        public override string ToString() => $"[{Tag:X4} {AaId} {AssertionInfo} {FinalChallenge} {KeyId} {Counters} {PubKey}]";
    }
}