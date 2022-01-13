using System;

namespace NHSOnline.Backend.PfsApi.Areas
{
    public static class JtiExtensions
    {
        public static string ToLoggableJti(this string jti)
        {
            if (string.IsNullOrWhiteSpace(jti))
            {
                return string.Empty;
            }

            var keyLengthToLog = Math.Min(jti.Length, 5);
            return jti[^keyLengthToLog..];
        }
    }
}