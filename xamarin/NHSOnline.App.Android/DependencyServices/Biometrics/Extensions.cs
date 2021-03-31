using System;

namespace NHSOnline.App.Droid.DependencyServices.Biometrics
{
    internal static class Extensions
    {
        internal static T ThrowIfNull<T>(this T? value, string message) where T:class
        {
            return value ?? throw new InvalidOperationException(message);
        }
    }
}