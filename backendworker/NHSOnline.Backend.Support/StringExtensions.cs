using System.Globalization;
using System.Linq;

namespace NHSOnline.Backend.Support
{
    public static class StringExtensions
    {
        public static string FormatToNhsNumber(this string sourceNhsNumber)
        {
            if (string.IsNullOrEmpty(sourceNhsNumber))
            {
                return "";
            }

            var filteredNhsNumber = sourceNhsNumber.RemoveWhiteSpace();

            // Belt and braces here, apparently the NHS number will always be 10 long,
            // if not, jut return whatever it is
            if (filteredNhsNumber.Length != 10)
            {
                return filteredNhsNumber;
            }

            return string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}",
                filteredNhsNumber.Substring(0, 3),
                filteredNhsNumber.Substring(3, 3),
                filteredNhsNumber.Substring(6, 4));
        }

        public static string RemoveWhiteSpace(this string sourceString)
        {
            return string.IsNullOrEmpty(sourceString) ? sourceString : string.Concat(sourceString.Where(c => !char.IsWhiteSpace(c)));
        }

        public static string EnquoteStringIfItContainsWhitespace(this string sourceString)
        {
            if (string.IsNullOrEmpty(sourceString))
            {
                return sourceString;
            }

            if (sourceString.Any(char.IsWhiteSpace))
            {
                return $"\"{sourceString}\"";
            }

            return sourceString;
        }
    }
}