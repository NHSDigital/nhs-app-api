using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.App.NhsLogin.Fido.Assertion.Models
{
    internal sealed class TagAuthenticateAssertionInfo
    {
        private const ushort Tag = 0x2E0E;

        private const byte UserHasBeenExplicitlyVerified = 0x01;
        private const ushort UafAlgSignSecp256R1EcdsaSha256Der = 0x02;

        private static ushort AuthenticatorVersion => 0;
        private static byte AuthenticationMode => UserHasBeenExplicitlyVerified;
        private static ushort SignatureAlgAndEncoding => UafAlgSignSecp256R1EcdsaSha256Der;

        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Consistent with other tags")]
        internal void Write(ITagLengthValueWriter writer)
        {
            using var valueWriter = writer.StartTag(Tag);

            valueWriter.Write(AuthenticatorVersion);
            valueWriter.Write(AuthenticationMode);
            valueWriter.Write(SignatureAlgAndEncoding);
        }

        public override string ToString() => $"[{Tag:X4} {AuthenticatorVersion:X4} {AuthenticationMode:X2} {SignatureAlgAndEncoding:X4}]";
    }
}