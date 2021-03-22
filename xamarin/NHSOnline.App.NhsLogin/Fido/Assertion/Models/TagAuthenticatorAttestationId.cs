using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace NHSOnline.App.NhsLogin.Fido.Assertion.Models
{
    internal sealed class TagAuthenticatorAttestationId
    {
        private const ushort Tag = 0x2E0B;

        private static readonly byte[] AuthenticatorAttestationId = Encoding.UTF8.GetBytes("EBA0#0001");

        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Consistent with other tags")]
        internal void Write(ITagLengthValueWriter writer)
        {
            using var valueWriter = writer.StartTag(Tag);

            valueWriter.Write(AuthenticatorAttestationId);
        }

        public override string ToString() => $"[{Tag:X4} {AuthenticatorAttestationId.ToHexString()}]";
    }
}