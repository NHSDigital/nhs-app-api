using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.App.NhsLogin.Fido.Assertion.Models
{
    internal sealed class TagRegisterAssertionInfo
    {
        private const ushort Tag = 0x2E0E;

        private const byte UserHasBeenExplicitlyVerified = 0x01;
        private const ushort UafAlgSignSecp256R1EcdsaSha256Der = 0x02;
        private const ushort UafAlgKeyEccX962Raw = 0x100;

        private static ushort AuthenticatorVersion => 0;
        private static byte AuthenticationMode => UserHasBeenExplicitlyVerified;
        private static ushort SignatureAlgAndEncoding => UafAlgSignSecp256R1EcdsaSha256Der;
        private static ushort PublicKeyAlgAndEncoding => UafAlgKeyEccX962Raw;

        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Consistent with other tags")]
        internal void Write(ITagLengthValueWriter writer)
        {
            using var valueWriter = writer.StartTag(Tag);

            valueWriter.Write(AuthenticatorVersion);
            valueWriter.Write(AuthenticationMode);
            valueWriter.Write(SignatureAlgAndEncoding);
            valueWriter.Write(PublicKeyAlgAndEncoding);
        }

        public override string ToString() => $"[{Tag:X4} {AuthenticatorVersion:X4} {AuthenticationMode:X2} {SignatureAlgAndEncoding:X4} {PublicKeyAlgAndEncoding:X4}]";
    }
}