using System;
using System.Globalization;
using System.Security.Cryptography;

namespace NHSOnline.HttpMocks.Support
{
    public static class UbrnGenerator
    {
        public static string NewUbrn() => $"{GetFourRandomDigits()}{GetFourRandomDigits()}{GetFourRandomDigits()}";

        private static string GetFourRandomDigits() =>
            RandomNumberGenerator.GetInt32(0, 10000).ToString("0000", CultureInfo.InvariantCulture);
    }
}