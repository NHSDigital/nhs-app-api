using System;

namespace NHSOnline.IntegrationTests.UI.Components
{
    internal static class XPathStringExtensions
    {
        internal static string QuoteXPathLiteral(this string text)
        {
            if (text.Contains('\'', StringComparison.Ordinal))
            {
                return $@"""{text}""";
            }

            return $"'{text}'";
        }
    }
}