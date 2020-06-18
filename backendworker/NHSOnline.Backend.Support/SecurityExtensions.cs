using System.Text.RegularExpressions;

namespace NHSOnline.Backend.Support
{
    public static class SecurityExtensions
    {
        public static bool IsSafeString(this string sourceString)
        {
            return !Regex.IsMatch(sourceString, "<(.|\n)*?>");
        }
    }
}
