using System;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    internal static class FocusableExtensions
    {
        public static bool IsNotTheSameAs(this IFocusable? first, IFocusable? second) => !first.IsTheSameAs(second);

        public static bool IsTheSameAs(this IFocusable? first, IFocusable? second)
            => string.Equals(first?.ElementDescription, second?.ElementDescription, StringComparison.Ordinal);
    }
}