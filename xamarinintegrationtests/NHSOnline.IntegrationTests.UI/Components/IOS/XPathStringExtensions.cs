using System;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    internal static class PredicateStringExtensions
    {
        internal static string QuotePredicateLiteral(this string text)
        {
            if (text.Contains('\'', StringComparison.Ordinal))
            {
                return $@"""{text}""";
            }

            return $"'{text}'";
        }
    }
}