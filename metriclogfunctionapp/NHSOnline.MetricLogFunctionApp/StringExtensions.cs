using System;
using System.Text;

namespace NHSOnline.MetricLogFunctionApp
{
    public static class StringExtensions
    {
        public static string DecodeBase64(this string value)
        {
            var valueBytes = Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(valueBytes);
        }
    }
}
