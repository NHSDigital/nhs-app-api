namespace NHSOnline.App.NhsLogin.Fido.Assertion.Models
{
    internal sealed class UserVerificationExtensionData
    {
        private const ushort Tag = 0x2E14;

        internal const uint UserVerifyPresence = 1;
        internal const uint UserVerifyFingerPrint = 2;
        internal const uint UserVerifyFacePrint = 3;

        internal const ushort KeyProtectionSoftware = 1;
        internal const ushort KeyProtectionSecureElement = 8;

        internal const ushort MatcherProtectionOnChip = 4;

        internal uint UserVerificationMethod { get; set; } = UserVerifyPresence + UserVerifyFingerPrint;
        internal ushort KeyProtection { get; set; } = KeyProtectionSoftware + KeyProtectionSecureElement;
        internal ushort MatcherProtection { get; set; } = MatcherProtectionOnChip;

        internal void Write(ITagLengthValueWriter writer)
        {
            using var valueWriter = writer.StartTag(Tag);

            valueWriter.Write(UserVerificationMethod);
            valueWriter.Write(KeyProtection);
            valueWriter.Write(MatcherProtection);
        }

        public override string ToString() => $"[{Tag:X4} {UserVerificationMethod:X8} {KeyProtection:X4} {MatcherProtection:X4}]";
    }
}