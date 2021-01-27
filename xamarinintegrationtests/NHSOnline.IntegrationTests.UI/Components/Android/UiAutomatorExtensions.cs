using System;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    internal static class UiAutomatorExtensions
    {
        internal static string QuoteUiAutomatorLiteral(this string text)
        {
            return @$"""{text.Replace(@"""", @"\""", StringComparison.Ordinal)}""";
        }
    }
}