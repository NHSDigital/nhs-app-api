using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace NHSOnline.App.NhsLogin.Fido.Assertion.Models
{
    internal sealed class UserVerificationExtensionId
    {
        private const ushort Tag = 0x2E13;

        private static readonly byte[] ExtensionId = Encoding.UTF8.GetBytes("fido.uaf.uvm");

        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Consistent with other tags")]
        internal void Write(ITagLengthValueWriter writer)
        {
            using var valueWriter = writer.StartTag(Tag);

            valueWriter.Write(ExtensionId);
        }

        public override string ToString() => $"[{Tag:X} {ExtensionId.ToHexString()}]";
    }
}