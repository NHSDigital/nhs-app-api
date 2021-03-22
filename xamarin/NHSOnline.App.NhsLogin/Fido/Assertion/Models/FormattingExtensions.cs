using System.Globalization;
using System.Linq;

namespace NHSOnline.App.NhsLogin.Fido.Assertion.Models
{
    internal static class FormattingExtensions
    {
        public static string ToHexString(this byte[] value)
            => string.Join(string.Empty, value.Select(b => b.ToString("X2", CultureInfo.InvariantCulture)));
    }
}