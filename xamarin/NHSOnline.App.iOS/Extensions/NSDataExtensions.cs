using System;
using Foundation;

namespace NHSOnline.App.iOS.Extensions
{
    public static class NSDataExtensions
    {
        public static string ToHexString(this NSData nsData)
            => BitConverter.ToString(nsData.ToArray()).Replace("-", "", StringComparison.Ordinal);
    }
}